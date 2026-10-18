using Microsoft.UI.Xaml;
using Microsoft.WindowsAppSDK.Runtime.Packages;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace AlertChangeIP.Classes
{
    public class AppParams  
    {
        private static readonly Lazy<AppParams> _instance = new Lazy<AppParams>(() => new AppParams());

        public static AppParams Instance => _instance.Value;

        private AppParams()
        {   
            Load();  
        }

        public          AppModeEnum     AppMode         = AppModeDefault;
        public static   AppModeEnum     AppModeDefault  = AppModeEnum.Ask;

        public const    string          DefaultWebURL = "https://api.ipify.org";
        public          string          WebURL = DefaultWebURL;
        
        public enum AppModeEnum
        {
            Ask = 0,
            Loop,
            Once
        }

        public static readonly List<string> WebURLList = new()
        {
            "https://api.ipify.org",
            "https://checkip.amazonaws.com",
            "https://ifconfig.me/ip",
            "https://icanhazip.com",
            "https://ipecho.net/plain",
            "https://ident.me",
            "https://myexternalip.com/raw",
            "https://wtfismyip.com/text",
            "https://ipinfo.io/ip"
        };    
        
        private static void Load()
        {
            string? json = AppParamsStorage.LoadSavedJson();

            if (json is not null)
            { 
                AppParams? temp = JsonSerializer.Deserialize<AppParams>(json);

                if (temp is not null)
                {
                    Instance.AppMode = temp.AppMode;
                    Instance.WebURL  = temp.WebURL;      
                }
            }
        }

        public void Save()
        {
            AppParamsStorage.SaveToJson(this);
        }
    }
}
