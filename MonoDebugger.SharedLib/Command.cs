using System.Runtime.Serialization;

namespace MonoDebugger.SharedLib
{
    [DataContract]
    public enum Command : byte
    {
        [EnumMember]
        DebugContent,
        [EnumMember]
        StartedMono,
        [EnumMember] 
        Shutdown
    }
}