using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections.Concurrent;

public class NetDataHandler<T> : Session.IDataHandler
{
    private T mOwner;
    private NetDataPool mNetDataPool;
    private object[] mCacheParameters = new object[2];
    private static Dictionary<byte, MethodInfo> msMethods = null;
    private ConcurrentQueue<Tuple<Session, NetData>> mMessageQueue = new ConcurrentQueue<Tuple<Session, NetData>>();

    static NetDataHandler()
    {
        BindMethods();
    }

    public NetDataHandler(T owner)
    {
        mOwner = owner;
        mNetDataPool = new NetDataPool();
    }

    /// <summary>
    /// 绑定函数处理
    /// </summary>
    static void BindMethods()
    {
        msMethods = new Dictionary<byte, MethodInfo>();
        foreach (var method in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var parameters = method.GetParameters();
            if (parameters.Length != 2 ||
                parameters[0].ParameterType != typeof(Session) &&
                !typeof(NetData).IsAssignableFrom(parameters[1].ParameterType))
                continue;

            var netData = Activator.CreateInstance(parameters[1].ParameterType) as NetData;
            msMethods[netData.DataId] = method;
        }
    }

    /// <summary>
    /// 处理网络消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="buffer"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public void OnData(Session session, byte[] buffer, int offset, int size)
    {
        using (var ser = new NetSerializer(buffer, offset, size))
        {
            // 取出消息的头
            var id = byte.MaxValue;
            ser.Ser(ref id);

            // 从消息池里面拿出来一个新消息，用来序列化
            if (!mNetDataPool.TryGet(id, out var netData))
            {
                Debug.LogError($"Fail to find netdata with id: {id}");
                return;
            }

            // 序列化出来数据
            ser.Ser(netData);

            // 添加到处理队列里面
            mMessageQueue.Enqueue(new Tuple<Session, NetData>(session, netData));
        }
    }

    /// <summary>
    /// 处理文本Json消息。
    /// </summary>
    /// <param name="session"></param>
    /// <param name="text"></param>
    public void OnText(Session session, string text)
    {
        var obj =  LitJson.JsonMapper.ToObject(text);
        var id = obj["Id"];

        // 文本消息这里只是取出一个数据拿来丢掉...:(
        // 因为后面json会新创建一个类型出来，用完之后会丢进去保存起来
        // 暂时为了不会造成内存问题，直接拿出去就丢掉吧
        if (!mNetDataPool.TryGet((byte)id, out var netData))
        {
            Debug.LogError($"Fail to find netdata with id: {id}");
            return;
        }

        // 序列化出来数据
        var data = obj["Data"];
        var json = data.ToJson();
        var newNetData = LitJson.JsonMapper.ToObject(netData.GetType(), json, false) as NetData;

        // 添加到处理队列里面
        mMessageQueue.Enqueue(new Tuple<Session, NetData>(session, newNetData));
    }

    /// <summary>
    /// 逻辑主线程来处理消息
    /// </summary>
    public void Update()
    {
        while (mMessageQueue.TryDequeue(out var item))
        {
            ProcessNetData(item.Item1, item.Item2);

            // 消息处理完成之后还回去
            mNetDataPool.Release(item.Item2);
        }
    }

    /// <summary>
    /// 处理网络消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="netData"></param>
    void ProcessNetData(Session session, NetData netData)
    {
        // 调用处理函数
        if (!msMethods.TryGetValue(netData.DataId, out var methodInfo))
        {
            Debug.LogError($"Fail to find handler for id: {netData.DataId}");
            return;
        }

        // 装载函数参数，执行函数调用
        mCacheParameters[0] = session;
        mCacheParameters[1] = netData;
        methodInfo.Invoke(mOwner, mCacheParameters);
    }
}
