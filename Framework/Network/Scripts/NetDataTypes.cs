using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class WhoAreYouData : NetData
{
    public override byte DataId => WhoAreYouId;
}

[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
public class PeerInfoData : NetData
{
    public string DeviceName;
    public string IpAddress;
    public override byte DataId => PeerInfoId;
}



[ProtoContract()]
public class ExportMessages
{
    [ProtoMember(1)]
    public WhoAreYouData WhoAreYouData;
    [ProtoMember(2)]
    public PeerInfoData PeerInfoData;

}