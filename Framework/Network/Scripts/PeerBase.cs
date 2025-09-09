using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 网络端点基类
/// </summary>
public abstract class PeerBase
{
    public bool LogData { get; set; } = false;

    protected NetSerializer mNetSerializer = new NetSerializer();
    protected List<Session> mSessions = new List<Session>();

    protected abstract void OnStart();
    protected abstract void OnUpdate();
    protected virtual void OnDestroy() { }
    protected virtual void OnClientClosed(Session session) { }

    public void Start()
    {
        OnStart();
    }

    public void Update()
    {
        OnUpdate();
    }

    public void Destroy()
    {
        for (var i = mSessions.Count - 1; i >= 0; i--)
            mSessions[i].Close();
        OnDestroy();
    }

    private void OnSessionClosed(Session session)
    {
        mSessions.Remove(session);
        OnClientClosed(session);
        session.OnClose();
    }

    /// <summary>
    /// 检查节点是否连接
    /// </summary>
    /// <param name="peerInfo"></param>
    /// <returns></returns>
    public bool IsPeerConnected(PeerInfo peerInfo)
    {
        foreach (var session in mSessions)
        {
            var remoteEndPoint = session.RemoteEndPoint as IPEndPoint;
            var remoteAddress = remoteEndPoint.Address.ToString();
            if (remoteAddress == peerInfo.IpAddress)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="netData"></param>
    public void SendData(NetData netData, Session session = null, bool binary = false)
    {
        if (binary)
        {
            mNetSerializer.Reset();
            mNetSerializer.WritePackage(netData);

            var buff = mNetSerializer.ToArray();
            if (session != null) session.Send(buff);
            else
            {
                foreach (var s in mSessions)
                    s.Send(buff);
            }
        }
        else
        {
            var package = new NetDataPackage { Id = netData.DataId, Data = netData };
            var json = LitJson.JsonMapper.ToJson(package);
            if (session != null) session.Send(json);
            else
            {
                foreach (var s in mSessions)
                    s.Send(json);
            }
        }
    }
}
