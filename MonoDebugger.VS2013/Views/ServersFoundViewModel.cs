using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using MonoDebugger.VS2013.MonoClient;
using MonoDebugger.VS2013.Settings;

namespace MonoDebugger.VS2013.Views
{
    public class ServersFoundViewModel
    {
        public ObservableCollection<MonoServerInformation> Servers { get; set; }
        public MonoServerInformation SelectedServer { get; set; }
        public string ManualIp { get; set; }
        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly UserSettingsManager userSettingsManager = new UserSettingsManager();

        public ServersFoundViewModel()
        {
            Servers = new ObservableCollection<MonoServerInformation>();
            var settings = userSettingsManager.Load();
            ManualIp = settings.LastIp;
            LookupServers(cts.Token);
        }

        private async void LookupServers(CancellationToken token)
        {
            var discovery = new MonoServerDiscovery();

            while (!token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
                var server = await discovery.SearchServer(token);
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
            var settings = userSettingsManager.Load();
            settings.LastIp = ManualIp;
            userSettingsManager.Save(settings);

            cts.Cancel();
        }
    }
}
