using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System;
using System.Reflection;

/// <summary>
/// 消息信息的对象池，注意这个是再多线程里面处理
/// 同一个类型的消息，在网络线程里面申请出来，序列化之后，加入消息队列
/// 在逻辑线程里面从消息队列里面把消息取出来之后，调用对应的处理函数，再返还回对象池
/// 这里需要处理一个问题，就是这个对象的数值是被序列化过了的，所以下次用的时候需要清除
/// </summary>
public class NetDataPool
{
    /// <summary>
    /// 初始化一个object的类型
    /// </summary>
    class ObjectInitializer<T>
    {
        private readonly Type mObjectType;
        private readonly FieldInfo[] mFields;

        public ObjectInitializer(Type type) 
        {
            mObjectType = type;
            mFields = type.GetFields();
        }

        /// <summary>
        /// 将一个object数值全部初始化掉
        /// </summary>
        /// <param name="obj"></param>
        public void Reset(object obj)
        {
            foreach (var field in mFields)
            {
                var defaultValue = field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null;
                field.SetValue(obj, defaultValue);
            }
        }
    }

    /// <summary>
    /// 根据类型区分的对象池
    /// </summary>
    class ObjectPool<T>
    {
        private readonly Type mObjectType;
        private ObjectInitializer<T> mInitializer;
        private ConcurrentQueue<T> mPool = new ConcurrentQueue<T>();

        public ObjectPool(Type type)
        { 
            mObjectType = type;
            mInitializer = new ObjectInitializer<T>(type);
        }

        /// <summary>
        /// 从对象池获取一个空闲的object
        /// </summary>
        /// <param name="reset">是否重置这个类型上的值</param>
        /// <returns></returns>
        public T Get(bool reset = true)
        {
            if (mPool.TryDequeue(out var obj))
            {
                if (reset) mInitializer.Reset(obj);
                return obj;
            }
            return (T)Activator.CreateInstance(mObjectType);
        }

        /// <summary>
        /// 这个object用完了还回来放到对象池里面。
        /// </summary>
        /// <param name="data"></param>
        public void Release(T data)
        {
            mPool.Enqueue(data);
        }
    }
    
    private Dictionary<byte, ObjectPool<NetData>> mObjectPools = new Dictionary<byte, ObjectPool<NetData>>();

    /// <summary>
    /// 构造函数里面对每个消息都新建
    /// </summary>
    public NetDataPool()
    {
        // 给每个消息类型id创建一个队列
        foreach (var netData in NetData.NetDatas)
            mObjectPools[netData.DataId] = new ObjectPool<NetData>(netData.GetType());
    }

    /// <summary>
    /// 根据消息id获取一个消息数据
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool TryGet(byte id, out NetData data)
    {
        data = null;
        if (!mObjectPools.TryGetValue(id, out var dataPool))
            return false;

        data = dataPool.Get(true);
        return true;
    }

    /// <summary>
    /// 这个消息用完了还回去
    /// </summary>
    /// <param name="data"></param>
    public void Release(NetData data)
    {
        if (mObjectPools.TryGetValue(data.DataId, out var dataPool))
            dataPool.Release(data);
    }
}
