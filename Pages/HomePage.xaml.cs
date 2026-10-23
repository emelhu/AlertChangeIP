using AlertChangeIP.Classes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System.Collections.ObjectModel;
using System.Net;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AlertChangeIP.Pages
{
    public sealed partial class HomePage : Page
    {
        #region ViewModel 
        private HomePageVM ViewModel;   
        #endregion

        public HomePage()
        {
            InitializeComponent();

            this.Loaded     += HomePage_Loaded;

            this.ViewModel   = new HomePageVM(AppParams.Instance);
            this.DataContext = this.ViewModel;

            HomePage.lastHomePageVM = this.ViewModel;
        }

        private void HomePage_Loaded(object sender, RoutedEventArgs e)
        {           
            this.ViewModel.RefreshView();
        }

        public static HomePageVM? lastHomePageVM = null;
    }

    public partial class HomePageVM : ObservableObject 
    {
        #region Area definition and common Fields 
        public Visibility showOnce { get; set; }
        public Visibility showLoop { get; set; }
        public Visibility showAsk  { get; set; }
        public Visibility showBoth { get; set; }                                                                                            // Once or Loop mode, show both IP addresses and read times

        [ObservableProperty] public partial string      actIP       { get; set; }
        [ObservableProperty] public partial string      lastRead    { get; set; }
        [ObservableProperty] public partial string      prevIP      { get; set; }
        [ObservableProperty] public partial string      prevRead    { get; set; }
        [ObservableProperty] public partial string      changedText { get; set; } = String.Empty;
        [ObservableProperty] public partial string      changedTextForeground { get; set; } = "Green";
        [ObservableProperty] public partial string      changedTextBackground { get; set; } = "MintCream";
        [ObservableProperty] public partial ObservableCollection<IPHistoryStruct>   ipHistory { get; set; } = new ObservableCollection<IPHistoryStruct>();
        #endregion

        #region Ask area Fields
        public string askText { get; init; }
        #endregion

        #region Once area Fields
        public string onceText { get; init; }
        #endregion

        #region Loop area Fields
        public string loopText { get; init; }
        #endregion

        #region Create ViewModel 
        private AppParams parameters;

        #pragma warning disable CS8618, CS9264
        public HomePageVM(AppParams appParams)
        {
            UpdateProperties(appParams);

            UpdateIPProperties();

            askText     = "To use the application, go to the Settings page and configure the operating parameters.";
            onceText    = "The application is running in Once mode. It will check the IP address once and then wait for the user's command.";
            loopText    = "The application is running in Loop mode. It will continuously check the IP address at regular intervals.";
        }
        #pragma warning restore CS8618, CS9264
        #endregion

        public void UpdateProperties(AppParams appParams)
        {
            parameters = appParams;

            #region ViewModel variables initialize
            showOnce = (parameters.AppMode == AppParams.AppModeEnum.Once) ? Visibility.Visible : Visibility.Collapsed;
            showLoop = (parameters.AppMode == AppParams.AppModeEnum.Loop) ? Visibility.Visible : Visibility.Collapsed;
            showAsk  = (parameters.AppMode == AppParams.AppModeEnum.Ask)  ? Visibility.Visible : Visibility.Collapsed;
            showBoth = (parameters.AppMode == AppParams.AppModeEnum.Once || parameters.AppMode == AppParams.AppModeEnum.Loop) ? Visibility.Visible : Visibility.Collapsed;
            #endregion
        }

        public void UpdateIPProperties()
        {
            #region Set ViewModel IP variables 
            actIP       = Helper.GetPublicIP(parameters.WebURL);                                                            // This should be set to the actual current IP address
            lastRead    = DateTime.Now.ToString("yyyy.MM.dd hh.mm.ss");                                                     // This should be set to the actual last read time
            prevIP      = "???";                                                                                            // This should be set to the actual previous IP address
            prevRead    = "???";                                                                                            // This should be set to the actual previous read time

            var tempList = IPHistoryStore.Read() ?? new List<IPHistoryStruct>();
            tempList.Reverse();                                                                                             // Reverse the list to show the most recent entries first

            if (tempList.Count > 0)                                                                                           
            {
                prevIP   = tempList[0].ipText;                                                                                             
                prevRead = tempList[0].whenText;

                if (prevIP == actIP)
                {
                    changedText           = "Identical IP";
                    changedTextForeground = "Green";
                    changedTextBackground = "MintCream";
                }
                else
                {
                    changedText           = "IP Changed!";
                    changedTextForeground = "Red";
                    changedTextBackground = "Yellow";
                    IPHistoryStore.Write(actIP);
                }
            }
            else
            {
                IPHistoryStore.Write(actIP);                                                                                              // If there is no history, write the current IP to the history
            }

            ipHistory = new ObservableCollection<IPHistoryStruct>(tempList);

            #endregion
        }


        #region Helpers  
        public void RefreshView()
        {
            OnPropertyChanged(string.Empty);
        }        
        #endregion
    }    
}

