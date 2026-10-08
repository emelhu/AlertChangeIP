using System;
using System.Collections.Generic;
using System.Text;

namespace AlertChangeIP.Classes
{
    public static class AppParams
    {
        public static AppMode appMode { get; set; } = AppMode.Ask;                                                                                  // Default value
        public enum AppMode
        {
            Ask = 0,
            Loop,
            Once
        }
    }
}
