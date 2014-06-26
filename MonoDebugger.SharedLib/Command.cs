using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MonoDebugger.SharedLib
{
    [DataContract]
    public enum Command : byte
    {
        [EnumMember]
        DebugContent,
        [EnumMember]
        StartedMono
    }
}
