using MonoDebugger.SharedLib;
using MonoDebugger.SharedLib.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.MonoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MonoLogger.Setup();

            using (var server = new MonoDebugServer())
            {
                server.StartAnnouncing();
                server.Start();

                server.WaitForExit();
            }
        }
    }
}
