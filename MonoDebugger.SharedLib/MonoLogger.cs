using System.IO;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace MonoDebugger.SharedLib
{
    public static class MonoLogger
    {
        public static string LoggerPath { get; private set; }

        public static void Setup()
        {
            var basePath = new FileInfo(typeof(MonoLogger).Assembly.Location).Directory.FullName;
            var logPath = Path.Combine(basePath, "Log");
            if (!Directory.Exists(logPath))
                Directory.CreateDirectory(logPath);
            LoggerPath = Path.Combine(logPath, "MonoDebugger.log");
            File.WriteAllText(basePath + "TEst.t", "qasd");
            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget { FileName = LoggerPath };
            config.AddTarget("file", fileTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, fileTarget));
            var console = new ColoredConsoleTarget();
            config.AddTarget("file", console);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, console));

            LogManager.Configuration = config;
        }
    }
}