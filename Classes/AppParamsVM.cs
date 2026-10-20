using CommunityToolkit.Mvvm.ComponentModel;                                                 // Install-Package CommunityToolkit.Mvvm
using CommunityToolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Text;
using System.Xml.Linq;

namespace AlertChangeIP.Classes
{
    public partial class AppParamsVM : ObservableObject 
    {
        #region Fields 
        [ObservableProperty]  public partial AppParams.AppModeEnum  AppMode { get; set; }

        [ObservableProperty] public partial string                  WebURL  { get; set; }

        [RelayCommand] public void SetWebURLDefault()
        {
            WebURL = AppParams.DefaultWebURL;
            OnPropertyChanged(nameof(WebURL));
        }

        [NotifyPropertyChangedFor(nameof(WebURLList))]
        [ObservableProperty] public partial bool ShowURLList { get; set; } = false;

        partial void OnShowURLListChanged(bool value)                                                               // OnXXXXXChanged  --> XXXX=PropertyName : Auto generated method
        {
            OnPropertyChanged(string.Empty);
        }

        [ObservableProperty] public partial bool Show2ndExpander { get; set; } = false;

        public  List<string> WebURLList { get; }   

        public IList<AppParams.AppModeEnum> AppModeEnumsList { get; }  
        #endregion


        #region Create ViewModel, Update Source  
        private AppParams sourceAppParams;

        public AppParamsVM(AppParams appParams)
        {
            sourceAppParams = appParams;

            //

            this.AppMode = appParams.AppMode;
            this.WebURL  = appParams.WebURL;

            //

            WebURLList       = new List<string>(AppParams.WebURLList);
            AppModeEnumsList = Enum.GetValues(typeof(AppParams.AppModeEnum)).Cast<AppParams.AppModeEnum>().ToList();
        }
        #endregion


        #region Helpers  
        public void RefreshView()
        {
            OnPropertyChanged(string.Empty);
        }

        public AppParams GetSource()
        {
            UpdateSource();
            return sourceAppParams;
        }

        public void UpdateSource()
        {
            sourceAppParams.AppMode = this.AppMode;
            sourceAppParams.WebURL  = this.WebURL;
        }

        public void SaveSource()
        {
            UpdateSource();
            sourceAppParams.Save();
        }
        #endregion
    }
}
