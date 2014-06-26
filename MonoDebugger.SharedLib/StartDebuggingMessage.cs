using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MonoDebugger.SharedLib
{
    [DataContract]
    public class StartDebuggingMessage
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public ApplicationType AppType { get; set; }

        [DataMember]
        public byte[] DebugContent { get; set; }
    }
}
