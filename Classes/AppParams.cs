using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.Text;

namespace AlertChangeIP.Classes
{
    public static class AppParams
    {
        public static AppMode appMode { get; set; } = AppMode.Ask;                                                                                  // Default value: Mode of app runing. Can be changed in SettingsPage.xaml.cs

        public const  string  defaultWebURL = "https://api.ipify.org";
        public static string  webURL { get; set; } = defaultWebURL;                                                                       // Default value: URL of WEB page to get public IP address. Can be changed in SettingsPage.xaml.cs
        public enum AppMode
        {
            Ask = 0,
            Loop,
            Once
        }        
    }
}
