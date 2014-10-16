using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace MonoDebugger.SharedLib.Server
{
    class Pdb2MdbGenerator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        internal void GeneratePdb2Mdb(string directoryName)
        {
            logger.Trace(directoryName);
            var files = Directory.GetFiles(directoryName, "*.dll").Concat(Directory.GetFiles(directoryName, "*.exe")).Where(x => !x.Contains("vshost"));
            logger.Trace(files.Count());

            DirectoryInfo dirInfo = new DirectoryInfo(directoryName);

            Parallel.ForEach(files, file =>
            {
                try
                {
                    var fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                    var pdbFile = Path.Combine(Path.GetDirectoryName(file), fileNameWithoutExt + ".pdb");
                    if (File.Exists(pdbFile))
                    {
                        logger.Trace("Generate mdp for: " + file);
                        ProcessStartInfo procInfo = new ProcessStartInfo(MonoUtils.GetPdb2MdbPath(), Path.GetFileName(file));
                        procInfo.WorkingDirectory = dirInfo.FullName;
                        procInfo.UseShellExecute = false;
                        procInfo.CreateNoWindow = true;
                        var proc = Process.Start(procInfo);
                        proc.WaitForExit();
                    }
                }
                catch (Exception ex)
                {
                    logger.Trace(ex);
                }
            });

            logger.Trace("Transformed Debuginformation pdb2mdb");
        }
    }
}
