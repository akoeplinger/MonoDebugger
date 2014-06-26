using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using MonoDebugger.SharedLib;
using MonoDebugger.VS2013.Debugger;
using MonoDebugger.VS2013.MonoClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace MonoDebugger.VS2013
{
    class MonoVisualStudioExtension
    {
        private DTE _dte;

        public MonoVisualStudioExtension(EnvDTE.DTE dTE)
        {
            _dte = dTE;
        }

        internal void BuildSolution()
        {
            var sb = (SolutionBuild2)_dte.Solution.SolutionBuild;
            sb.Build(true);
        }

        internal string GetStartupAssemblyPath()
        {
            var startupProject = GetStartupProject();
            return GetAssemblyPath(startupProject);
        }

        private Project GetStartupProject()
        {
            var sb = (SolutionBuild2)_dte.Solution.SolutionBuild;
            var project = ((Array)sb.StartupProjects).Cast<string>().First();
            var startupProject = _dte.Solution.Item(project);
            return startupProject;
        }

        internal string GetAssemblyPath(Project vsProject)
        {
            string fullPath = vsProject.Properties.Item("FullPath").Value.ToString();
            string outputPath = vsProject.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
            string outputDir = Path.Combine(fullPath, outputPath);
            string outputFileName = vsProject.Properties.Item("OutputFileName").Value.ToString();
            string assemblyPath = Path.Combine(outputDir, outputFileName);
            return assemblyPath;
        }


        internal async System.Threading.Tasks.Task AttachDebugger(string ipAddress)
        {
            var path = GetStartupAssemblyPath();
            var targetExe = Path.GetFileName(path);
            var outputDirectory = Path.GetDirectoryName(path);

            var startup = GetStartupProject();

            bool isWeb = ((object[])startup.ExtenderNames).Any(x => x.ToString() == "WebApplication");
            var appType = isWeb ? ApplicationType.Webapplication : ApplicationType.Desktopapplication;
            if (appType == ApplicationType.Webapplication)
                outputDirectory += @"\..\..\";

            var client = new DebugClient(appType, targetExe, outputDirectory);
            var session = await client.ConnectToServerAsync(ipAddress);
            await session.TransferFilesAsync();
            await session.WaitForAnswerAsync();

            var pInfo = GetDebugInfo(ipAddress, targetExe, outputDirectory);
            var sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_dte);
            try
            {
                IVsDebugger dbg = (IVsDebugger)sp.GetService(typeof(SVsShellDebugger));
                int hr = dbg.LaunchDebugTargets(1, pInfo);
                Marshal.ThrowExceptionForHR(hr);
            }
            catch
            {
                string msg;
                IVsUIShell sh = (IVsUIShell)sp.GetService(typeof(SVsUIShell));
                sh.GetErrorInfo(out msg);
                throw;
            }
            finally
            {
                if (pInfo != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pInfo);
            }
        }

        private IntPtr GetDebugInfo(string args, string targetExe, string outputDirectory)
        {
            VsDebugTargetInfo info = new VsDebugTargetInfo();
            info.cbSize = (uint)Marshal.SizeOf(info);
            info.dlo = DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;

            info.bstrExe = Path.Combine(outputDirectory, targetExe);
            info.bstrCurDir = outputDirectory;
            info.bstrArg = args; // no command line parameters
            info.bstrRemoteMachine = null; // debug locally
            info.grfLaunch = (uint)__VSDBGLAUNCHFLAGS.DBGLAUNCH_StopDebuggingOnEnd;
            info.fSendStdoutToOutputWindow = 0;
            info.clsidCustom = MonoGuids.EngineGuid;
            info.grfLaunch = 0;

            IntPtr pInfo = Marshal.AllocCoTaskMem((int)info.cbSize);
            Marshal.StructureToPtr(info, pInfo, false);
            return pInfo;
        }
    }
}
