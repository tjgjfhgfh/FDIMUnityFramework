using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public abstract class NetData
{
    public const byte WhoAreYouId = 5;
    public const byte PeerInfoId = 6;

    private static Dictionary<byte, NetData> msNetDataMap;
    public static IEnumerable<NetData> NetDatas => msNetDataMap.Values;

    /// <summary>
    /// 静态构造函数自动注册所有的消息类型。
    /// </summary>
    static NetData()
    {
        msNetDataMap = Assembly.GetAssembly(typeof(NetData)).GetTypes()
            .Where(type => typeof(NetData).IsAssignableFrom(type) && type != typeof(NetData) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as NetData)
            .ToDictionary(netData => netData.DataId, netData => netData);
    }

    public abstract byte DataId { get; }
}


/// <summary>
/// 单独发送的json文件
/// </summary>
public class NetDataPackage
{
    public byte Id;
    public NetData Data;
}