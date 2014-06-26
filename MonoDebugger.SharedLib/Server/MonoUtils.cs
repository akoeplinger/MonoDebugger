using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoDebugger.SharedLib.Server
{
    internal static class MonoUtils
    {
        public static string GetMonoPath()
        {
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return "mono";
            }

            return Path.Combine(GetMonoRootPathWindows(), @"bin\mono.exe");
        }

        public static string GetMonoXsp4()
        {
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return "xsp4";
            }

            return Path.Combine(GetMonoRootPathWindows(), @"bin\Xsp4.bat");
        }

        public static string GetPdb2MdbPath()
        {
            int p = (int)Environment.OSVersion.Platform;
            if ((p == 4) || (p == 6) || (p == 128))
            {
                return "pdb2mdb";
            }

            return Path.Combine(GetMonoRootPathWindows(), @"bin\pdb2mdb.bat");
        }

        private static string GetMonoRootPathWindows()
        {
            try
            {
                var localMachine = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
                var monoKey = localMachine.OpenSubKey(@"Software\Novell\Mono\");
                var monoVersion = monoKey.GetValue("DefaultCLR") as string;
                var versionKey = localMachine.OpenSubKey(string.Format(@"Software\Novell\Mono\{0}", monoVersion));
                var path = (string)versionKey.GetValue("SdkInstallRoot");
                return path;
            }
            catch
            {
            }
            return string.Empty;
        }
    }
}
