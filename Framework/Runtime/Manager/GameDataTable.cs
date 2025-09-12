using System;
using System.Collections.Generic;
using ProtoBuf;

namespace FDIM.Framework
{
    [ProtoContract]
    public class TextruePath
    {
        [ProtoMember(1)]
        public int Id { get; set; }
    
        [ProtoMember(2)]
        public string TextrueUrl { get; set; }
    
    }
    
    [ProtoContract]
    public partial class GameData
    {
        [ProtoMember(1)]
        public List<TextruePath> TextruePath { get; set; }  = new List<TextruePath>();
    
    }
    
}
