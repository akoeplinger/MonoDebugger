using MonoDebugger.SharedLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib.Server
{
    class ClientSession
    {
        private string ZipFileName { get { return _directoryName + ".zip"; } }

        private string _root = Path.Combine(Path.GetTempPath(), "MonoDebugger");
        private string _directoryName;
        private string _targetExe;
        private Process _proc;
        private TcpCommunication _communication;
        private IPAddress _remoteEndpoint;

        public ClientSession(Socket socket)
        {
            _directoryName = Path.Combine(_root, Path.GetRandomFileName());
            _remoteEndpoint = ((IPEndPoint)socket.RemoteEndPoint).Address;
            _communication = new TcpCommunication(socket);

            if(!Directory.Exists("MonoDebugger"))
                Directory.CreateDirectory("MonoDebugger");
        }

        public void HandleSession()
        {
            try
            {
                Trace.WriteLine(string.Format("New Session from {0}", _remoteEndpoint));

                while (_communication.IsConnected)
                {
                    if (_proc != null && _proc.HasExited)
                        return;

                    MessageBase msg = _communication.Receive();

                    switch (msg.Command)
                    {
                        case Command.DebugContent:
                            StartDebugging((StartDebuggingMessage)msg.Payload);
                            _communication.Send(Command.StartedMono, new StatusMessage { });
                            break;
                    }
                }
            }
            catch (SocketException socketEx)
            {
                if (_proc != null && !_proc.HasExited)
                    _proc.Kill();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private void StartDebugging(StartDebuggingMessage msg)
        {
            if (!Directory.Exists(_root))
                Directory.CreateDirectory(_root);

            _targetExe = msg.FileName;

            Trace.WriteLine(string.Format("Receiving content from {0}", _remoteEndpoint));
            File.WriteAllBytes(ZipFileName, msg.DebugContent);
            ZipFile.ExtractToDirectory(ZipFileName, _directoryName);

            foreach (var file in Directory.GetFiles(_directoryName, "*vshost*"))
                File.Delete(file);

            File.Delete(ZipFileName);
            Trace.WriteLine(string.Format("Extracted content from {0} to {1}", _remoteEndpoint, _directoryName));

            var generator = new Pdb2MdbGenerator();
            var binaryDirectory = msg.AppType == ApplicationType.Desktopapplication ? _directoryName : Path.Combine(_directoryName, "bin");
            generator.GeneratePdb2Mdb(binaryDirectory);

            StartMono(msg.AppType);
        }

        private void StartMono(ApplicationType type)
        {
            MonoProcess proc = MonoProcess.Start(type, _targetExe);
            proc.ProcessStarted += MonoProcessStarted;
            _proc = proc.Start(_directoryName);
            _proc.EnableRaisingEvents = true;
            _proc.Exited += _proc_Exited;
        }

        private void MonoProcessStarted(object sender, EventArgs e)
        {
            var web = sender as MonoWebProcess;
            if (web != null)
            {
                Process.Start(web.Url);
            }
        }

        void _proc_Exited(object sender, EventArgs e)
        {
            Console.WriteLine("Program closed: " + _proc.ExitCode);
            try
            {
                Directory.Delete(_directoryName, true);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(string.Format("Cant delete {0} - {1}", _directoryName, ex.Message));
            }
        }
    }
}
