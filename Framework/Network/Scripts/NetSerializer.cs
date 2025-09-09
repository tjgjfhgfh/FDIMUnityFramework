using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetSerializer : IDisposable
{
    MemoryStream mMemoryStream = null;
    BinaryReader mBinaryReader = null;
    BinaryWriter mBinaryWriter = null;

    public bool Writing => mBinaryWriter != null;

    public long Length => mMemoryStream.Length;
    public long Position => mMemoryStream.Position;

    public NetSerializer()
    {
        mMemoryStream = new MemoryStream();
        mBinaryWriter = new BinaryWriter(mMemoryStream);
    }

    public NetSerializer(byte[] buffer, int index, int size)
    {
        mMemoryStream = new MemoryStream(buffer, index, size, false);
        mBinaryReader = new BinaryReader(mMemoryStream);
    }

    public void Dispose()
    {
        if (mBinaryReader != null) mBinaryReader.Close();
        if (mBinaryWriter != null) mBinaryWriter.Close();
        if (mMemoryStream != null) mMemoryStream.Close();
    }

    /// <summary>
    /// 获取写入的Buffer
    /// </summary>
    /// <returns></returns>
    public byte[] ToArray()
    {
        return mMemoryStream.ToArray();
    }

    public NetSerializer Ser(ref byte data)
    {
        if (Writing) mBinaryWriter.Write(data);
        else data = mBinaryReader.ReadByte();
        return this;
    }

    public NetSerializer Ser(ref Int32 data)
    {
        if (Writing) mBinaryWriter.Write(data);
        else data = mBinaryReader.ReadInt32();
        return this;
    }

    public NetSerializer Ser(ref float data)
    {
        if (Writing) mBinaryWriter.Write(data);
        else data = mBinaryReader.ReadSingle();
        return this;
    }

    public NetSerializer Ser(ref Vector3 data)
    {
        Ser(ref data.x).Ser(ref data.y).Ser(ref data.z);
        return this;
    }

    public NetSerializer Ser(ref Quaternion data)
    {
        Ser(ref data.x).Ser(ref data.y).Ser(ref data.z).Ser(ref data.w);
        return this;
    }

    /// <summary>
    /// 设置读写位置
    /// </summary>
    /// <param name="pos"></param>
    public void Seek(long pos)
    {
        mMemoryStream.Seek(pos, SeekOrigin.Begin);
    }

    /// <summary>
    /// 重置一下
    /// </summary>
    public void Reset()
    {
        mMemoryStream.Position = 0;
        mMemoryStream.SetLength(0);
    }

    public NetSerializer Ser(ref string data)
    {
        if (Writing) mBinaryWriter.Write(data);
        else data = mBinaryReader.ReadString();
        return this;
    }

    public NetSerializer Ser(ref UInt16 data)
    {
        if (Writing) mBinaryWriter.Write(data);
        else data = mBinaryReader.ReadUInt16();
        return this;
    }

    public NetSerializer Ser(NetData data)
    {
        if (Writing)
            ProtoBuf.Serializer.Serialize(mMemoryStream, data);
        else
            ProtoBuf.Meta.RuntimeTypeModel.Default.Deserialize(mMemoryStream, data, data.GetType());
        return this;
    }

    /// <summary>
    /// 写入网络包裹
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public NetSerializer WritePackage(NetData netData)
    {
        // 执行写入数据
        var cmdCode = netData.DataId;
        Ser(ref cmdCode);
        Ser(netData);

        return this;
    }
}
