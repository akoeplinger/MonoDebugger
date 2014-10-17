using System.Runtime.Serialization;

namespace MonoDebugger.SharedLib
{
    [DataContract]
    public enum ApplicationType
    {
        [EnumMember] Desktopapplication,
        [EnumMember] Webapplication
    }
}