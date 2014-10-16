using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using MonoDebugger.VS2013.MonoClient;

namespace MonoDebugger.VS2013.Views
{
    public class ServersFoundViewModel
    {
        public ObservableCollection<MonoServerInformation> Servers { get; set; }
        public MonoServerInformation SelectedServer { get; set; }
        public string ManualIp { get; set; }
        private volatile bool _lookupServers;

        public ServersFoundViewModel()
        {
            Servers = new ObservableCollection<MonoServerInformation>();
            LookupServers();
        }

        private async void LookupServers()
        {
            var discovery = new MonoServerDiscovery();
            _lookupServers = true;
            while (_lookupServers)
            {
                var server = await discovery.SearchServer();
                if (server != null)
                {
                    var exists = Servers.FirstOrDefault(x => IPAddress.Equals(x.IpAddress, server.IpAddress));
                    if (exists == null)
                    {
                        Servers.Add(server);
                        server.LastMessage = DateTime.Now;
                    }
                    else
                    {
                        exists.LastMessage = DateTime.Now;
                    }
                }

                foreach (var deadServer in Servers.Where(x => ((DateTime.Now - x.LastMessage).TotalSeconds > 5)).ToList())
                    Servers.Remove(deadServer);
            }
        }

        public void StopLooking()
        {
            _lookupServers = false;
        }
    }
}
