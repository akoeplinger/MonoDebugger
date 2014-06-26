using MonoDebugger.SharedLib;
using MonoDebugger.SharedLib.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.MonoClient
{
    public class DebugClient
    {
        public string TargetExe { get; set; }
        public string OutputDirectory { get; set; }
        public IPAddress CurrentServer { get; private set; }

        private ApplicationType _type;

        public DebugClient(ApplicationType type, string targetExe, string outputDirectory)
        {
            _type = type;
            TargetExe = targetExe;
            OutputDirectory = outputDirectory;
        }

        public async Task<DebugSession> ConnectToServerAsync(string ipAddress)
        {
            CurrentServer = IPAddress.Parse(ipAddress);
            
            TcpClient tcp = new TcpClient();
            await tcp.ConnectAsync(CurrentServer, MonoDebugServer.TcpPort);
            return new DebugSession(this, _type, tcp.Client);
        }

    }
}
