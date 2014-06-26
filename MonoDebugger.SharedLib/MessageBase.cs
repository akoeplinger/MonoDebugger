using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MonoDebugger.SharedLib
{
    [DataContract]
    public class MessageBase
    {
        [DataMember]
        public Command Command { get; set; }

        [DataMember]
        public object Payload { get; set; }
    }
}
