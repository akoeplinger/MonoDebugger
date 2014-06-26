using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.SharedLib
{
    public static class ZipFile
    {
        public static void CreateFromDirectory(string directory, string targetZip)
        {
            using (var zip = new Ionic.Zip.ZipFile())
            {
                zip.AddDirectory(directory);
                zip.Save(targetZip);
            }
        }

        public static void ExtractToDirectory(string ZipFileName, string targetDirectory)
        {
            using (var zip = Ionic.Zip.ZipFile.Read(ZipFileName))
            {
                foreach (var e in zip)
                {
                    e.Extract(targetDirectory);
                }
            }
        }
    }
}
