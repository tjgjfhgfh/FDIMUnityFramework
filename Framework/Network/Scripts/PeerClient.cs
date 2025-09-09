using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using WebSocketSharp;

public class PeerClient<T> : PeerBase
{
    private List<PeerInfo> mPeerList = new List<PeerInfo>();
    private NetDataHandler<T> mDataHandler;
    private WebSocketSharp.WebSocket mWebSocket;
    private Session mSession;

    public bool IsConnecting { private set; get; } = false;
    public bool HostConnected { private set; get; } = false;
    public IReadOnlyList<PeerInfo> Servers => mPeerList;

    public PeerClient(T owner)
    {
        mDataHandler = new NetDataHandler<T>(owner);
    }

    protected override void OnStart()
    {
    }

    protected override void OnUpdate()
    {
        mDataHandler?.Update();
    }

    protected override void OnDestroy()
    {
    }

    protected override void OnClientClosed(Session session)
    {
        base.OnClientClosed(session);
        mSession = null;
        HostConnected = false;
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="peerInfo"></param>
    public void ConnectWebsocket(PeerInfo peerInfo)
    {
        try
        {
            IsConnecting = true;

            var url = $"ws://{peerInfo.IpAddress}:{peerInfo.TcpPort}/{peerInfo.Service}";
            if (LogData) Debug.Log($"ConnectWebsocket: url={url}");
            mWebSocket = new WebSocketSharp.WebSocket(url);
            mWebSocket.OnOpen += OnSessionOpen;
            mWebSocket.OnMessage += OnMessage;
            mWebSocket.ConnectAsync();
        }
        catch (Exception ex)
        {
            IsConnecting = false;
            Debug.LogException(ex);
        }
        finally
        {
        }
    }

    private void OnMessage(object sender, MessageEventArgs e)
    {
        if (mSession == null)
        {
            Debug.LogError($"Session丢失情况下收到网络消息。");
            return;
        }

        if (e.IsText)
        {
            if (LogData) Debug.Log($"收到文本：{e.Data}");
            mSession.OnMessage(e.Data);
        }
        else if (e.IsBinary)
        {
            if (LogData) Debug.Log($"收到数据：{string.Join(" ", e.RawData)}");
            mSession.OnMessage(e.RawData);
        }
    }

    private void OnSessionOpen(object sender, EventArgs e)
    {
        mSession = new Session(mDataHandler, new WebsocketSessionWrapper(mWebSocket, LogData));
        mSessions.Add(mSession);

        // connected to a host.
        HostConnected = true;
    }

    /// <summary>
    /// 做一个简单的封装给session来发送消息。
    /// </summary>
    class WebsocketSessionWrapper : Session.IDataSender
    {
        private WebSocket mWebSocket;
        public bool LogData;

        public WebsocketSessionWrapper(WebSocket webSocket, bool logData)
        {
            mWebSocket = webSocket;
            LogData = logData;
        }

        public void SendData(byte[] buffer)
        {
            if (LogData) Debug.Log($"发送数据：{string.Join(" ", buffer)}");
            mWebSocket.Send(buffer);
        }

        public void SendText(string text)
        {
            if (LogData) Debug.Log($"发送消息：{text}");
            mWebSocket.Send(text);
        }

        public void SendClose()
        {
            mWebSocket.Close();
        }
    }
}
