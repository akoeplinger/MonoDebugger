using Microsoft.Win32;
using MonoDebugger.VS2013.Debugger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MonoDebugger.Installer
{
    public static class MonoDebuggerInstaller
    {
        private const string ENGINE_PATH = @"SOFTWARE\Microsoft\VisualStudio\{0}\AD7Metrics\Engine\";
        private const string CLSID_PATH = @"SOFTWARE\Microsoft\VisualStudio\{0}\CLSID\";

        public static void RegisterDebugEngine()
        {
            #if DEBUG
            RegisterVersion("12.0Exp");
            #else
            RegisterVersion("12.0");
            #endif
        }

        private static void RegisterVersion(string version)
        {
            string dllPath = @"C:\Users\Christian\AppData\Local\Microsoft\VisualStudio\12.0Exp\Extensions\Giesswein-Apps\MonoDebugger\0.1.1\MonoDebugger.VS2013.Debugger.dll";

            List<RegistryKey> rootKeys = new List<RegistryKey>();
            rootKeys.Add(Registry.LocalMachine);
            rootKeys.Add(Registry.CurrentUser);

            foreach (RegistryKey rootKey in rootKeys)
            {
                string configSufix = rootKey == Registry.CurrentUser ? "_Config" : string.Empty;
                string enginePath = string.Format(ENGINE_PATH, version + configSufix);
                string clsidPath = string.Format(CLSID_PATH, version + configSufix);

                using (RegistryKey engineKey = rootKey.CreateSubKey(enginePath + MonoGuids.EngineGuid.ToString("B").ToUpper()))
                {
                    engineKey.SetValue("CLSID", MonoGuids.EngineGuid.ToString("B").ToUpper());
                    engineKey.SetValue("ProgramProvider", MonoGuids.ProgramProviderGuid.ToString("B").ToUpper());
                    engineKey.SetValue("Attach", 1, RegistryValueKind.DWord);
                    engineKey.SetValue("AddressBP", 0, RegistryValueKind.DWord);
                    engineKey.SetValue("AutoSelectPriority", 4, RegistryValueKind.DWord);
                    engineKey.SetValue("CallstackBP", 1, RegistryValueKind.DWord);
                    engineKey.SetValue("Name", MonoGuids.EngineName);
                    engineKey.SetValue("PortSupplier", MonoGuids.ProgramProviderGuid.ToString("B").ToUpper());
                    engineKey.SetValue("AlwaysLoadLocal", 1, RegistryValueKind.DWord);
                }

                using (var clsidKey = rootKey.CreateSubKey(clsidPath + MonoGuids.EngineGuid.ToString("B").ToUpper()))
                {
                    clsidKey.SetValue("Assembly", Assembly.GetExecutingAssembly().GetName().Name);
                    clsidKey.SetValue("Class", "MonoDebugger.VS2013.Debugger.MonoEngine");
                    clsidKey.SetValue("InprocServer32", @"c:\windows\system32\mscoree.dll");
                    clsidKey.SetValue("CodeBase", dllPath);
                }

                using (var programProviderKey = rootKey.CreateSubKey(clsidPath + MonoGuids.ProgramProviderGuid.ToString("B").ToUpper()))
                {
                    programProviderKey.SetValue("Assembly", Assembly.GetExecutingAssembly().GetName().Name);
                    programProviderKey.SetValue("Class", "MonoDebugger.VS2013.Debugger.MonoProgramProvider");
                    programProviderKey.SetValue("InprocServer32", @"c:\windows\system32\mscoree.dll");
                    programProviderKey.SetValue("CodeBase", dllPath);
                }
            }
        }
    }
}
