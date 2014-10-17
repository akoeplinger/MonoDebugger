using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace MonoDebugger.SharedLib
{
    public static class MonoLogger
    {
        public static string LoggerPath { get; private set; }

        public static void Setup()
        {
            LoggerPath = Path.Combine(Directory.GetCurrentDirectory(), "MonoDebugger.log");

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget { FileName = "MonoDebugger.log" };
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
            var console = new ColoredConsoleTarget();
            config.AddTarget("file", console);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, console));

            LogManager.Configuration = config;
        }
    }
}