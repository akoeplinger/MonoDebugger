using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib.Server
{
    class MonoWebProcess : MonoProcess
    {
        public string Url { get; private set; }

        internal override Process Start(string workingDirectory)
        {
            var monoBin = MonoUtils.GetMonoXsp4();
            var args = GetProcessArgs();
            ProcessStartInfo procInfo = GetProcessStartInfo(workingDirectory, monoBin);

            procInfo.CreateNoWindow = true;
            procInfo.UseShellExecute = false;
            procInfo.EnvironmentVariables["MONO_OPTIONS"] = args;
            procInfo.RedirectStandardOutput = true;

            _proc = Process.Start(procInfo);
            Task.Run(() =>
            {
                while (!_proc.StandardOutput.EndOfStream)
                {
                    string line = _proc.StandardOutput.ReadLine();

                    if (line.StartsWith("Listening on address"))
                    {
                        var url = line.Substring(line.IndexOf(":") + 2).Trim();
                        if (url == "0.0.0.0")
                            Url = "localhost";
                        else
                            Url = url;
                    }
                    else if (line.StartsWith("Listening on port"))
                    {
                        var port = line.Substring(line.IndexOf(":") + 2).Trim();
                        port = port.Substring(0, port.IndexOf(" "));
                        Url += ":" + port;

                        if (line.Contains("non-secure"))
                            Url = "http://" + Url;
                        else
                            Url = "https://" + Url;

                        RaiseProcessStarted();
                    }


                    Trace.WriteLine(line);
                }
            });

            return _proc;
        }
    }
}
