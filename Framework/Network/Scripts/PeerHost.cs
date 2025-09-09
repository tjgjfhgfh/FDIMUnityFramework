using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

/// <summary>
/// 主机执行处理
/// </summary>
public class PeerHost<T> : PeerBase
{
    public readonly int TcpPort;
    public readonly string Service;

    private WebSocketServer mWebSocketServer;
    private NetDataHandler<T> mDataHandler;

    public Action<Session> OnSessionConnected;
    public Action<Session> OnSessionClosed;

    public PeerHost(T owner, int tcpPort, string service)
    {
        TcpPort = tcpPort;
        Service = service;
        mDataHandler = new NetDataHandler<T>(owner);
    }

    protected override void OnStart()
    {
        // 启动TCP服务器来监听，获取别人连接过来的。
        StartWebSocketServer("0.0.0.0", TcpPort, Service);//, tcpClient => mClients.Add(new Session(tcpClient, mDataHandler)));
    }

    protected override void OnUpdate()
    {
        mDataHandler.Update();
    }

    protected override void OnClientClosed(Session session)
    {
        base.OnClientClosed(session);

        OnSessionClosed?.Invoke(session);
    }

    /// <summary>
    /// 销毁处理
    /// </summary>
    protected override void OnDestroy()
    {
        mWebSocketServer?.Stop();
        mWebSocketServer = null;

        base.OnDestroy();
    }

    /// <summary>
    /// StartTcpServer
    /// </summary>
    /// <param name="port"></param>
    void StartWebSocketServer(string ip, int port, string service)
    {
        try
        {
            Debug.Log("StartWebSocketServer: " + port);
            mWebSocketServer = new WebSocketServer($"ws://{ip}:{port}");
            mWebSocketServer.KeepClean = false; // 关闭自动清理超时连接
            mWebSocketServer.Log.Level = LogLevel.Error;
            mWebSocketServer.Log.Output = (data, file) => Debug.Log(data.ToString());
            mWebSocketServer.AddWebSocketService<InternalWebSocketService>($"/{service}", OnClientConnected);
            mWebSocketServer.Start();
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 收到了新的TCP客户端请求
    /// </summary>
    /// <param name="tcpClient"></param>
    private void OnClientConnected(InternalWebSocketService service)
    {
        service.Session = new Session(mDataHandler, service);
        service.LogData = LogData;
        service.IgnoreExtensions = true;//
        service.OnClosed = session => OnSessionClosed(session);

        mSessions.Add(service.Session);
        OnSessionConnected?.Invoke(service.Session);
    }

    /// <summary>
    /// 内部默认的消息通讯服务
    /// 通过websocket协议来收发消息
    /// </summary>
    class InternalWebSocketService : WebSocketBehavior, Session.IDataSender
    {
        public Session Session { get; set; }
        public bool LogData = false;

        public Action<Session> OnClosed;

        protected override void OnOpen()
        {
            base.OnOpen();

            Session.RemoteEndPoint = UserEndPoint;

            if (LogData) Debug.Log($"连接建立: {Session.RemoteEndPoint}");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            base.OnMessage(e);

            if (e.IsText)
            {
                if (LogData) Debug.Log($"收到文本：{e.Data}");
                Session.OnMessage(e.Data);
            }
            else if (e.IsBinary)
            {
                if (LogData) Debug.Log($"收到数据：{string.Join(" ", e.RawData)}");
                Session.OnMessage(e.RawData);
            }
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);

            OnClosed?.Invoke(Session);

            if (LogData) Debug.Log($"连接关闭：{e.Reason}({e.Code})");
        }

        public void SendData(byte[] buffer)
        {
            if (LogData) Debug.Log($"发送数据：{string.Join(" ", buffer)}");
            if (ReadyState == WebSocketState.Open)
                Send(buffer);
        }

        public void SendText(string text)
        {
            if (LogData) Debug.Log($"发送文本：{text}");
            if (ReadyState == WebSocketState.Open)
                Send(text);
        }

        public void SendClose()
        {
            Close();
        }
    }
}
