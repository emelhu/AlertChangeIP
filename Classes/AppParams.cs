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

        public          AppModeEnum     AppMode { get; set; } = AppModeDefault;                                                 // { get; set; } to JasonSerializer
        public static   AppModeEnum     AppModeDefault  = AppModeEnum.Ask;

        public const    string          DefaultWebURL = "https://api.ipify.org";
        public          string          WebURL { get; set; } = DefaultWebURL;                                                   // { get; set; } to JasonSerializer

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
        
        private void Load()
        {
            AppParamsDTO? temp = AppParamsStorage.LoadFromJson();
            temp?.ApplyTo(this);                                                                                        // Apply the loaded values to the singleton instance           
        }

        public void Save()
        {
            AppParamsStorage.SaveToJson(AppParamsDTO.From(this));
        }
    }

    public class AppParamsDTO                                                                                               // Data Transfer Object for AppParams because AppParams is a singleton and cannot be instantiated directly for serialization/deserialization
    {
        public AppParams.AppModeEnum AppMode { get; set; } = AppParams.AppModeDefault;
        public string                WebURL  { get; set; } = AppParams.DefaultWebURL;

        //

        public static AppParamsDTO From(AppParams src)
        {
            AppParamsDTO ret = new();

            ret.AppMode = src.AppMode;
            ret.WebURL  = src.WebURL;

            return ret;
        }

        public void ApplyTo(AppParams target)
        {
            target.AppMode = this.AppMode;
            target.WebURL  = this.WebURL;
        }
    }
}