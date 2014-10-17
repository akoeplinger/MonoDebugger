using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.MonoClient
{
    public class MonoServerDiscovery
    {
        public async Task<MonoServerInformation> SearchServer(CancellationToken token)
        {
            using (var udp = new UdpClient(new IPEndPoint(IPAddress.Any, 15000)))
            {
                var result = await Task.WhenAny(udp.ReceiveAsync(), Task.Delay(500, token));
                var task = result as Task<UdpReceiveResult>;
                if (task != null)
                {
                    var udpResult = task.Result;
                    var msg = Encoding.Default.GetString(udpResult.Buffer);
                    return new MonoServerInformation { Message = msg, IpAddress = udpResult.RemoteEndPoint.Address };
                }

                return null;
            }
        }
    }
}
