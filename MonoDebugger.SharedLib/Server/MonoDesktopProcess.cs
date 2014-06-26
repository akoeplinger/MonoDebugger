using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib.Server
{
    class MonoDesktopProcess : MonoProcess
    {
        private string _targetExe;

        public MonoDesktopProcess(string targetExe)
        {
            _targetExe = targetExe;
        }

        internal override Process Start(string workingDirectory)
        {
            var monoBin = MonoUtils.GetMonoPath();
            DirectoryInfo dirInfo = new DirectoryInfo(workingDirectory);

            var args = GetProcessArgs();
            ProcessStartInfo procInfo = GetProcessStartInfo(workingDirectory, monoBin);
            procInfo.Arguments = args + " \"" + _targetExe + "\"";

            _proc = Process.Start(procInfo);
            RaiseProcessStarted();
            return _proc;
        }
    }
}
