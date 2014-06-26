using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using System.Windows.Interop;
using System.Windows;
using EnvDTE80;
using System.Collections.Generic;
using System.IO;
using MonoDebugger.VS2013.Debugger;
using MonoDebugger.SharedLib.Server;
using System.Reflection;

namespace MonoDebugger.VS2013
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad("f1536ef8-92ec-443c-9ed7-fdadf150da82")]
    [Guid(GuidList.guidMonoDebugger_VS2013PkgString)]
    public sealed class MonoDebugger_VS2013Package : Package, IOleCommandTarget
    {
        private MonoDebugServer _server = new MonoDebugServer();
        private MonoVisualStudioExtension _monoExtension;

        public MonoDebugger_VS2013Package()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            var dte = (DTE)GetService(typeof(DTE));
            _monoExtension = new MonoVisualStudioExtension(dte);
            TryRegisterAssembly(dte.RegistryRoot);


            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/MonoDebugger.VS2013;component/Resources/Resources.xaml", UriKind.Relative) });

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (mcs != null)
            {
                CommandID debugLocally = new CommandID(GuidList.guidMonoDebugger_VS2013CmdSet, (int)PkgCmdIDList.cmdLocalDebugCode);
                var localCmd = new OleMenuCommand(DebugLocalClicked, debugLocally);
                localCmd.BeforeQueryStatus += cmd_BeforeQueryStatus;
                mcs.AddCommand(localCmd);


                CommandID menuCommandID = new CommandID(GuidList.guidMonoDebugger_VS2013CmdSet, (int)PkgCmdIDList.cmdRemodeDebugCode);
                var cmd = new OleMenuCommand(DebugRemoteClicked, menuCommandID);
                cmd.BeforeQueryStatus += cmd_BeforeQueryStatus;
                mcs.AddCommand(cmd);
            }
        }

        private void TryRegisterAssembly(string registryRoot)
        {
            try
            {
                var regKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\{8BF3AB9F-3864-449A-93AB-E7B0935FC8F5}");

#if !DEBUG
                if (regKey != null)
                    return;
#endif
                string location = typeof(DebuggedMonoProcess).Assembly.Location;

                string regasm = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe";
                if (!Environment.Is64BitOperatingSystem)
                    regasm = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\RegAsm.exe";

                var p = new ProcessStartInfo(regasm, location);
                p.Verb = "runas";
                p.RedirectStandardOutput = true;
                p.UseShellExecute = false;
                p.CreateNoWindow = true;

                var proc = System.Diagnostics.Process.Start(p);
                while (!proc.HasExited)
                {
                    string txt = proc.StandardOutput.ReadToEnd();
                }

                using (var config = VSRegistry.RegistryRoot(__VsLocalRegistryType.RegType_Configuration))
                {
                    MonoDebuggerInstaller.RegisterDebugEngine(location, config);
                }
            }
            catch (UnauthorizedAccessException unauthorized)
            {
                MessageBox.Show("Failed finish installation of MonoDebugger - Please run Visual Studio once als Administrator...", "MonoDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch
            {
            }
        }

        void cmd_BeforeQueryStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                var dte = GetService(typeof(DTE)) as DTE;
                var sb = (SolutionBuild2)dte.Solution.SolutionBuild;
                menuCommand.Visible = sb.StartupProjects != null;
                if (menuCommand.Visible)
                    menuCommand.Enabled = ((Array)sb.StartupProjects).Cast<string>().Count() == 1;
            }
        }

        private void DebugLocalClicked(object sender, EventArgs e)
        {
            StartLocalServer();
        }

        private async void StartLocalServer()
        {
            try
            {
                if (!_server.IsRunning)
                {
                    _server.StartAsync();
                }

                _monoExtension.BuildSolution();
                await _monoExtension.AttachDebugger(MonoDebugger.SharedLib.Server.MonoProcess.GetLocalIp().ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                MessageBox.Show(ex.Message, "MonoDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DebugRemoteClicked(object sender, EventArgs e)
        {
            StartSearching();
        }

        private async void StartSearching()
        {
            ServersFound dlg = new ServersFound();

            if (dlg.ShowDialog().GetValueOrDefault())
            {
                try
                {
                    _monoExtension.BuildSolution();
                    await _monoExtension.AttachDebugger(dlg.ViewModel.SelectedServer.IpAddress.ToString());
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    MessageBox.Show(ex.Message, "MonoDebugger", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
