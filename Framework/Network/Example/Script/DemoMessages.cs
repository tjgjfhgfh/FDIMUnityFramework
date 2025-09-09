using ProtoBuf;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DemoStringEvent : UnityEvent<string> { }

public abstract class DemoMessageBase : NetData
{
    public const byte HeartBeatDataId = 9;
    public const byte TestMessage = 10;
    public const byte ReplyMessage = 11;
    public const byte TrackerId = 12;
    public const byte HandTrackerId = 13;
    public const byte ProbeDataId = 14;
    public const byte PacketMetadataId = 15;
    public const byte CalibrationDataId = 16;
    public const byte CalibrationErrorDataId = 17;
    public const byte ModelDataId = 18;
    public const byte PointCloudDataId = 19;
    public const byte TrackerDataPacketProtoId = 20;




}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class DemoMessage : DemoMessageBase
{
    public string Message;

    public override byte DataId => TestMessage;
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class HeartBeatData : DemoMessageBase
{

    public bool Status;
    public override byte DataId => HeartBeatDataId;

}

/// <summary>
/// 头部的位置
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class TrackerDataPacketNew : DemoMessageBase
{

    public Vector3_t position1;


    public Quaternion_t rotation1;

    public Vector3_t position2;


    public Quaternion_t rotation2;

    public double timestamp;
    public bool hasTwoObjects;

    public override byte DataId => TrackerId;
}

/// <summary>
/// 基本的数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class PacketMetadata : DemoMessageBase
{

    public float timestamp;
    public string deviceName;

    public override byte DataId => PacketMetadataId;
}


/// <summary>
/// 手部tracker数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class HandTracker : DemoMessageBase
{

    public Vector3_t trackerPosition;
    public Quaternion_t trackerRotation;
    public bool trackerHasValidPosition;
    public override byte DataId => HandTrackerId;
}


/// <summary>
/// 探针数据
/// </summary>

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class ProbeData : DemoMessageBase
{

    public bool hasProbeData;
    public Vector3_t probePosition;
    public Vector3_t probeDirection;
    public float distanceToTarget;
    public bool isDirectionCorrect;
    public bool hasReachedTarget;
    public float angleToTarget;
    public Vector3_t targetPosition;

    //角度数据
    public float xAxisAngle;
    public float yAxisAngle;
    public float zAxisAngle;
    public override byte DataId => ProbeDataId;
}

/// <summary>
/// 校准数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class CalibrationData : DemoMessageBase
{

    public bool isCalibrated;
    public Matrix4x4_t calibrationMatrix;
    public bool hasMountingOffset;
    public Vector3_t mountingOffset;
    public Quaternion_t mountingRotationOffset;
    public override byte DataId => CalibrationDataId;
}


/// <summary>
/// 注册误差数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class CalibrationErrorData : DemoMessageBase
{

    public bool hasRegistrationData;
    public float registrationRMSE;
    public Quaternion_t registrationRotation;
    public Vector3_t registrationTranslation;
    public override byte DataId => CalibrationErrorDataId;
}


/// <summary>
/// 模型数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class ModelDataProto : DemoMessageBase
{

    public bool hasModelData;
    public Vector3_t modelPosition;
    public Quaternion_t modelRotation;
    public Vector3_t modelScale;
    public override byte DataId => ModelDataId;
}

/// <summary>
/// 点云数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class PointCloudData : DemoMessageBase
{

    public bool hasModelData;
    public Vector3_t modelPosition;
    public Quaternion_t modelRotation;
    public Vector3_t modelScale;
    public override byte DataId => PointCloudDataId;
}

/// <summary>
/// TrackerDataSenderSteamVRNew里面的，好像是tracker的数据
/// </summary>
[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class TrackerDataPacketProto : DemoMessageBase
{

    public Vector3_t position;
    public Quaternion_t rotation;
    public float timestamp;
    public override byte DataId => TrackerDataPacketProtoId;
}


[ProtoContract]
public struct Vector3_t
{
    [ProtoMember(1)] public float x;
    [ProtoMember(2)] public float y;
    [ProtoMember(3)] public float z;

    public static implicit operator Vector3(Vector3_t v) => new Vector3(v.x, v.y, v.z);
    public static implicit operator Vector3_t(Vector3 v) => new Vector3_t { x = v.x, y = v.y, z = v.z };
}

[ProtoContract]
public struct Quaternion_t
{
    [ProtoMember(1)] public float x;
    [ProtoMember(2)] public float y;
    [ProtoMember(3)] public float z;
    [ProtoMember(4)] public float w;

    public static implicit operator Quaternion(Quaternion_t q) => new Quaternion(q.x, q.y, q.z, q.w);
    public static implicit operator Quaternion_t(Quaternion q) => new Quaternion_t { x = q.x, y = q.y, z = q.z, w = q.w };
}

[ProtoContract]
public struct Matrix4x4_t
{
    [ProtoMember(1)] public float M00;
    [ProtoMember(2)] public float M01;
    [ProtoMember(3)] public float M02;
    [ProtoMember(4)] public float M03;
    [ProtoMember(5)] public float M10;
    [ProtoMember(6)] public float M11;
    [ProtoMember(7)] public float M12;
    [ProtoMember(8)] public float M13;
    [ProtoMember(9)] public float M20;
    [ProtoMember(10)] public float M21;
    [ProtoMember(11)] public float M22;
    [ProtoMember(12)] public float M23;
    [ProtoMember(13)] public float M30;
    [ProtoMember(14)] public float M31;
    [ProtoMember(15)] public float M32;
    [ProtoMember(16)] public float M33;

    // 从 Matrix4x4_t 转回 UnityEngine.Matrix4x4
    public static implicit operator Matrix4x4(Matrix4x4_t m)
    {
        var mat = new Matrix4x4();
        mat.m00 = m.M00; mat.m01 = m.M01; mat.m02 = m.M02; mat.m03 = m.M03;
        mat.m10 = m.M10; mat.m11 = m.M11; mat.m12 = m.M12; mat.m13 = m.M13;
        mat.m20 = m.M20; mat.m21 = m.M21; mat.m22 = m.M22; mat.m23 = m.M23;
        mat.m30 = m.M30; mat.m31 = m.M31; mat.m32 = m.M32; mat.m33 = m.M33;
        return mat;
    }
    public static implicit operator Matrix4x4_t(UnityEngine.Matrix4x4 m)
    {
        return new Matrix4x4_t
        {
            M00 = m.m00,
            M01 = m.m01,
            M02 = m.m02,
            M03 = m.m03,
            M10 = m.m10,
            M11 = m.m11,
            M12 = m.m12,
            M13 = m.m13,
            M20 = m.m20,
            M21 = m.m21,
            M22 = m.m22,
            M23 = m.m23,
            M30 = m.m30,
            M31 = m.m31,
            M32 = m.m32,
            M33 = m.m33
        };
    }
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class DemoReply : DemoMessageBase
{
    public string Reply;

    public override byte DataId => ReplyMessage;
}