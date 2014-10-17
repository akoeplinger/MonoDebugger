using System;
using System.IO;
using Newtonsoft.Json;
using NLog;

namespace MonoDebugger.VS2013.Settings
{
    public class UserSettingsManager
    {
        private static readonly string settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Settings.json");
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public UserSettings Load()
        {
            var result = new UserSettings(); 
            if (File.Exists(settingsPath))
            {
                try
                {
                    var content = File.ReadAllText(settingsPath);
                    result = JsonConvert.DeserializeObject<UserSettings>(content);
                    return result;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            return result;
        }

        public void Save(UserSettings settings)
        {
            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(settingsPath, json);
        }
    }
}