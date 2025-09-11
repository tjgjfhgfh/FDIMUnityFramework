using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class DemoClient : MonoBehaviour
{
    public string IpAddress = "";
    public int Port = 3066;
    public string Service = "Default";
    public bool LogData = true;
    public bool BinaryData = false;
    public DemoStringEvent OnMessageEvent;

    private PeerClient<DemoClient> mClient;

    // Start is called before the first frame update
    // 并发扫描量
    private const int MAX_CONCURRENT = 10;

    void Start()
    {
        // 一启动就开始扫描局域网
        StartCoroutine(ScanAndConnect());
    }

    void Update()
    {
        mClient?.Update();
    }

    void OnDestroy()
    {
        mClient?.Destroy();
        mClient = null;
    }

    IEnumerator ScanAndConnect()
    {
        // 获取本机局域网段前缀，比如 "192.168.1."
        var localIP = GetLocalIPAddress();
        if (localIP == null) yield break;
        string prefix = localIP.Substring(0, localIP.LastIndexOf('.') + 1);

        var toScan = new Queue<string>();
        for (int i = 1; i <= 254; i++)
            toScan.Enqueue(prefix + i);

        int active = 0;
        bool found = false;

        while (toScan.Count > 0 && !found)
        {
            if (active < MAX_CONCURRENT)
            {
                string ip = toScan.Dequeue();
                active++;
                // 并发尝试连接
                StartCoroutine(TryConnect(ip, Port, 0.2f, successIp =>
                {
                    active--;
                    if (!found && successIp != null)
                    {
                        found = true;
                        Debug.Log($"[Scanner] 找到服务端：{successIp}:{Port}");
                        BeginWebSocket(successIp);
                    }
                }));
            }
            else
            {
                // 达到并发上限，等一帧再继续
                yield return null;
            }
        }
    }

    // 用短连接测试端口是否开放
    IEnumerator TryConnect(string ip, int port, float timeout, Action<string> callback)
    {
        var tcp = new TcpClient();
        var connect = tcp.BeginConnect(ip, port, null, null);
        float start = Time.time;
        while (!connect.IsCompleted && Time.time - start < timeout)
            yield return null;

        bool ok = connect.IsCompleted && tcp.Connected;
        tcp.Close();

        callback(ok ? ip : null);
    }

    // 真正用 PeerClient 建立 WebSocket
    void BeginWebSocket(string ip)
    {
        mClient = new PeerClient<DemoClient>(this);
        mClient.LogData = LogData;
        mClient.ConnectWebsocket(new PeerInfo
        {
            IpAddress = ip,
            TcpPort = Port,
            Service = Service
        });
    }

    // 获取本机 IPv4
    string GetLocalIPAddress()
    {
        foreach (var ni in System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != System.Net.NetworkInformation.OperationalStatus.Up) continue;
            foreach (var ua in ni.GetIPProperties().UnicastAddresses)
            {
                if (ua.Address.AddressFamily == AddressFamily.InterNetwork &&
                    !IPAddress.IsLoopback(ua.Address))
                    return ua.Address.ToString();
            }
        }
        return null;
    }

    public void SetBinaryData(bool binary)
    {
        Debug.Log($"切换消息模式：binary={binary}");
        BinaryData = binary;
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    public void ConnectToServer()
    {
        if (mClient != null) 
            throw new System.Exception($"已经处于连接中");

        mClient = new PeerClient<DemoClient>(this);
        mClient.LogData = LogData;
        mClient.ConnectWebsocket(new PeerInfo { IpAddress = IpAddress, TcpPort = Port, Service = Service });
    }

    /// <summary>
    /// 发送文本demo消息过去
    /// </summary>
    /// <param name="message"></param>
    /// <exception cref="System.Exception"></exception>
    public void SendTextMessage(string message)
    {
        if (mClient == null)
            throw new System.Exception($"未连接");
        mClient.SendData(new DemoMessage { Message = message }, binary: BinaryData);
    }

    /// <summary>
    /// 收到消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="demoMessage"></param>
    public void OnDemoMessage(Session session, DemoMessage demoMessage)
    {
        OnMessageEvent?.Invoke(demoMessage.Message);
    }

    /// <summary>
    /// 服务器发送过来的回复消息。
    /// </summary>
    /// <param name="session"></param>
    /// <param name="demoReply"></param>
    public void OnDemoReplay(Session session, DemoReply demoReply)
    {
        OnMessageEvent?.Invoke(demoReply.Reply);
    }
}
