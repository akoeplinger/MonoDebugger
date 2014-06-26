using MonoDebugger.SharedLib.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.MonoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());
            var server = new MonoDebugServer();
            server.StartAnnouncing();
            server.Start();
        }
    }
}
