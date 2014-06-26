using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.VS2013.MonoClient
{
    public class MonoServerDiscovery
    {
        public async Task<MonoServerInformation> SearchServer()
        {
            using (UdpClient udp = new UdpClient(15000))
            {
                var result = await Task.WhenAny(udp.ReceiveAsync(), Task.Delay(500));
                if (result is Task<UdpReceiveResult>)
                {
                    var udpResult = ((Task<UdpReceiveResult>)result).Result;
                    var msg = Encoding.Default.GetString(udpResult.Buffer);
                    return new MonoServerInformation { Message = msg, IpAddress = udpResult.RemoteEndPoint.Address };
                }

                return null;
            }
        }
    }
}
