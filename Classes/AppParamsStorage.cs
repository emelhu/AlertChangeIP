using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

using static AlertChangeIP.Classes.AppParams;

namespace AlertChangeIP.Classes
{
    internal static class AppParamsStorage  
    {
        private static string filePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlertChangeIP", "AppParams.json");            // %LOCALAPPDATA%\AlertChangeIP\AppParams.json

        public static void SaveToJson(AppParams appParams)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);

            if (directoryPath is not null && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            //

            try
            {
                var json = JsonSerializer.Serialize(appParams);
                File.WriteAllText(filePath, json);
            }
            catch
            {
                LoggingService.WriteLog("Failed to save AppParams to file.", LoggingService.LogLevel.Error);
            }
        }    

        public static string? LoadSavedJson()
        {
            if (File.Exists(filePath)) 
            {                 
                try
                {
                    var json = File.ReadAllText(filePath);

                    return json;
                }
                catch
                {
                    LoggingService.WriteLog("Failed to load AppParams from file.", LoggingService.LogLevel.Error);
                }
            }

            return null;
        }        
    }
}
