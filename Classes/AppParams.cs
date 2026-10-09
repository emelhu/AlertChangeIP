using Microsoft.UI.Xaml;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace AlertChangeIP.Classes
{
    public class AppParams : INotifyPropertyChanged
    {
        //public AppParams()
        //{
        //    _appMode = appModeDefault;
        //}

        private AppMode _appMode = appModeDefault;
        public AppMode appMode
        {
            get => _appMode;
            set
            {
                if (_appMode != value)
                {
                    _appMode       = value;
                    appModeDefault = value;                                         // The next time you instantiate the class, this should be the default value.

                    OnPropertyChanged();
                }
            }
        }

        public static AppMode appModeDefault = AppMode.Ask;

        public const  string  defaultWebURL = "https://api.ipify.org";
        private string _webURL = defaultWebURL;
        public string  webURL
        {
            get => _webURL;
            set
            {
                if (_webURL != value)                                                 // because: when edit-field changed (user see other text) but value change only when exit edit-field.
                {
                    _webURL = value;
                    OnPropertyChanged();
                }
            }
        }

        public enum AppMode
        {
            Ask = 0,
            Loop,
            Once
        }

        

        public static readonly List<string> webURLList = new()
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

        #region INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion

        public void webURLChange(string newValue)
        {
            webURL = newValue;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("webURL"));                          // Must because: when edit-field changed (user see other text) but value change only when exit edit-field.
        }
    }
}
