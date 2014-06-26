using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoDebugger.Installer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Registering MonoDebugger...");
            MonoDebuggerInstaller.RegisterDebugEngine();
            Console.WriteLine("Finished!");
            Thread.Sleep(5000);
        }
    }
}
