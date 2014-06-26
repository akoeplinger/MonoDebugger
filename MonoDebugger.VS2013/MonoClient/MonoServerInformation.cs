using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoDebugger.VS2013.MonoClient
{
    public class MonoServerInformation
    {
        public System.Net.IPAddress IpAddress { get; set; }
        
        public string Message { get; set; }

        public DateTime LastMessage { get; set; }
    }
}
