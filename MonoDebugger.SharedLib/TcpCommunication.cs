using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib
{
    public class TcpCommunication
    {
        public bool IsConnected { get { return _socket.IsSocketConnected(); } }

        private Socket _socket;
        private DataContractSerializer _serializer;

        public TcpCommunication(Socket socket)
        {
            _socket = socket;
            var contracts = GetType().Assembly.GetTypes().Where(x => x.GetCustomAttributes(typeof(DataContractAttribute), true).Any()).ToList();
            _serializer = new DataContractSerializer(typeof(MessageBase), contracts);
        }

        public void Send(Command cmd, object payload)
        {
            using (var ms = new MemoryStream())
            {
                _serializer.WriteObject(ms, new MessageBase { Command = cmd, Payload = payload });
                var buffer = ms.ToArray();
                _socket.Send(BitConverter.GetBytes(buffer.Length));
                _socket.Send(buffer);
            }
        }

        public MessageBase Receive()
        {
            var buffer = new byte[sizeof(int)];
            int received = _socket.Receive(buffer);
            var size = BitConverter.ToInt32(buffer, 0);
            return ReceiveContent(size);
        }

        private MessageBase ReceiveContent(int size)
        {
            using (var ms = new MemoryStream())
            {
                int totalReceived = 0;
                while (totalReceived != size)
                {
                    var buffer = new byte[Math.Min(1024 * 10, size - totalReceived)];
                    int received = _socket.Receive(buffer);
                    totalReceived += received;
                    ms.Write(buffer, 0, received);
                }

                ms.Seek(0, SeekOrigin.Begin);
                return _serializer.ReadObject(ms) as MessageBase;
            }
        }

        public Task<MessageBase> ReceiveAsync()
        {
            return Task.Factory.StartNew(() => Receive());
        }

        public void Disconnect()
        {
            if (_socket != null)
            {
                _socket.Close(1);
                _socket.Dispose();
            }
        }
    }
}
