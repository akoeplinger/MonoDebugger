using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib.Server
{
    public class MonoDebugServer
    {
        public const int TcpPort = 13001;
        public bool IsRunning { get { return _isRunning; } }

        private volatile bool _isRunning;
        private Task _announceTask;
        private TcpListener _tcp;
        private Task _listeningTask;

        public void Start()
        {
            StartAsync().Wait();
        }

        public Task StartAsync()
        {
            InitTcp();
            _listeningTask = Task.Factory.StartNew(StartListening);
            return _listeningTask;
        }

        private void InitTcp()
        {
            _tcp = new TcpListener(IPAddress.Any, TcpPort);
            _tcp.Start();
            _isRunning = true;
        }

        private void StartListening()
        {
            while (_isRunning)
            {
                var client = _tcp.AcceptTcpClient();
                var clientSession = new ClientSession(client.Client);
                Task.Factory.StartNew(clientSession.HandleSession);
            }
        }

        public void Stop()
        {
            _isRunning = false;
            Task.WaitAll(_listeningTask);
        }

        public void StartAnnouncing()
        {
            _isRunning = true;
            _announceTask = Task.Factory.StartNew(() =>
            {
                try
                {
                    Trace.WriteLine("Start announcing");
                    UdpClient client = new UdpClient();
                    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 15000);
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

                    while (_isRunning)
                    {
                        byte[] bytes = Encoding.ASCII.GetBytes("MonoServer");
                        client.Send(bytes, bytes.Length, ip);
                        Thread.Sleep(100);
                    }

                    Trace.WriteLine("Stopping announcing");
                    client.Close();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            });
        }
    }
}
