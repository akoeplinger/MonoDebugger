using MonoDebugger.SharedLib;
using MonoDebugger.SharedLib.Server;

namespace MonoDebugger.MonoServer
{
    internal class Program
    {
        private static void Main(string[] args)
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