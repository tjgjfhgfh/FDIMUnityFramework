using System;
using UnityEngine;

public class Session
{
    private IDataHandler mDataHandler = null;
    private IDataSender mDataSender = null;

    public System.Net.IPEndPoint RemoteEndPoint;

    public interface IDataHandler
    {
        void OnData(Session session, byte[] buffer, int offset, int size);
        void OnText(Session session, string text);
    }

    public interface IDataSender
    {
        void SendData(byte[] buffer);
        void SendText(string text);
        void SendClose();
    }

    public Session(IDataHandler dataHandler, IDataSender dataSender)
    {
        mDataHandler = dataHandler;
        mDataSender = dataSender;
    }

    /// <summary>
    /// 主动关闭session
    /// 发送关闭消息
    /// </summary>
    public void Close()
    {
        mDataSender.SendClose();
    }

    public void OnClose()
    {

    }

    /// <summary>
    /// 收到二进制消息
    /// </summary>
    /// <param name="buffer"></param>
    public void OnMessage(byte[] buffer)
    {
        try
        {
            mDataHandler.OnData(this, buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 收到文本消息
    /// </summary>
    /// <param name="message"></param>
    public void OnMessage(string  message)
    {
        try
        {
            mDataHandler.OnText(this, message);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// 发送二进制数据
    /// </summary>
    /// <param name="buffer"></param>
    public void Send(byte[] buffer)
    {
        mDataSender.SendData(buffer);
    }

    /// <summary>
    /// 发送文本数据
    /// </summary>
    /// <param name="message"></param>
    public void Send(string message)
    {
        mDataSender.SendText(message);
    }
}
