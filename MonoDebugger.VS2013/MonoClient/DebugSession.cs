using MonoDebugger.SharedLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.MonoClient
{
    public class DebugSession
    {
        public DebugClient Client { get; private set; }
        private TcpCommunication _communication;
        private ApplicationType _type;

        public DebugSession(DebugClient debugClient, ApplicationType type, Socket socket)
        {
            Client = debugClient;
            _type = type;
            _communication = new TcpCommunication(socket);
        }

        public void TransferFiles()
        {
            DirectoryInfo info = new DirectoryInfo(Client.OutputDirectory);
            if (!info.Exists)
                throw new DirectoryNotFoundException("Directory not found");

            var targetZip = Path.Combine(info.FullName, "DebugContent.zip");
            if (File.Exists(targetZip))
                File.Delete(targetZip);

            ZipFile.CreateFromDirectory(info.FullName, targetZip);

            _communication.Send(Command.DebugContent, new StartDebuggingMessage
            {
                AppType = _type,
                DebugContent = File.ReadAllBytes(targetZip),
                FileName = Client.TargetExe
            });

            File.Delete(targetZip);
            Console.WriteLine("Finished transmitting");
        }

        public Task TransferFilesAsync()
        {
            return Task.Factory.StartNew(TransferFiles);
        }

        public async Task WaitForAnswerAsync()
        {
            var delay = Task.Delay(8000);
            var msg = await Task.WhenAny(_communication.ReceiveAsync(), delay);
            
            if (msg is Task<MessageBase>)
                return;

            if (msg == delay)
                throw new Exception("Did not receive an answer in time...");
            throw new Exception("Cant start debugging");
        }
    }
}
