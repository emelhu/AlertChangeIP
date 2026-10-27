using AlertChangeIP.Classes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {           
            this.ViewModel.RefreshView();

            // 

            // bool answer = await Helper.ShowDialogAsync(this, "Which action would you like to take? 'Close' or 'Hide' the window?", "'Close' or 'Hide' the window?", "Close", "Hide");

            if (AppParams.Instance.AppMode == AppParams.AppModeEnum.Loop)
            {  // Handle Loop mode specific logic here
                AppParams.Instance.handleWindowClose = true;                                                                // Window.Closed() -> for Close or Hide question                                                                                                 

                await Task.Delay(1000).ContinueWith(_ =>
                {
                    this.ViewModel.autoMinimizeToTrayIkon = true;                                                           // because start process clear it with property changes
                });

                 await Task.Delay(10000).ContinueWith(_ =>                                                                  // If the user doesn't make any changes, it will automatically go to the background after 10 seconds
                {
                    var app = (App)Application.Current;
                    app.mainWindowInstance?.HideToTray();
                }); 
            }
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

        [ObservableProperty] public partial string      actIP       { get; set; } = String.Empty;
        [ObservableProperty] public partial string      lastRead    { get; set; } = String.Empty;
        [ObservableProperty] public partial string      prevIP      { get; set; } = String.Empty;
        [ObservableProperty] public partial string      prevRead    { get; set; } = String.Empty;
        [ObservableProperty] public partial string      changedText { get; set; } = String.Empty;
        [ObservableProperty] public partial string      message     { get; set; } = String.Empty;
        [ObservableProperty] public partial Brush       changedTextForeground { get; set; } = new SolidColorBrush(Colors.Green);    
        [ObservableProperty] public partial Brush       changedTextBackground { get; set; } = new SolidColorBrush(Colors.MintCream);
        [ObservableProperty] public partial ObservableCollection<IPHistoryStruct>   ipHistory { get; set; } = new ObservableCollection<IPHistoryStruct>();
        [ObservableProperty] public partial bool        showIPHistory       { get; set; } = false;
        [ObservableProperty] public partial bool        forcesSaveIPButton  { get; set; } = false;

        partial void OnshowIPHistoryChanged(bool value)
        {                                                                                               // Visibility="{x:Bind ViewModel.showIPHistory, Converter={StaticResource BoolToVisibilityConverter}}"    Visibility="{x:Bind ViewModel.visibilityIPHistory, Mode=TwoWay}"
            visibilityIPHistory = value ? Visibility.Visible : Visibility.Collapsed;
            RefreshView();
        }
        [ObservableProperty] public partial Visibility visibilityIPHistory { get; set; } = Visibility.Collapsed;
        #endregion

        #region Ask area Fields
        public string askText { get; init; } = String.Empty;
        #endregion

        #region Once area Fields
        public string onceText { get; init; } = String.Empty;   
        #endregion

        #region Loop area Fields
        public string loopText { get; init; } = String.Empty;
        #endregion

        #region  Once & Loop area Buttons
        [RelayCommand] private async Task CheckIPButtonClick()
        {   // This method is called when the "Check IP" button is clicked in the UI
            var oldIP = actIP;

            UpdateIPProperties();

            if (actIP == oldIP)
            {
                await ShowMessageAsync("The IP address has not changed, same as before.");
            }
            else
            {
                await ShowMessageAsync("The IP address has changed! Please save it if necessary.");
            }
        }

        [RelayCommand] private async Task SaveIPButtonClick()
        {   // This method is called when the "Save IP" button is clicked in the UI           

            if ((actIP != prevIP) || forcesSaveIPButton)
            { 
                IPHistoryStore.Write(actIP);

                UpdateIPProperties();                                                                                                            

                await ShowMessageAsync("The IP address has saved!");
            }
            else
            {
                await ShowMessageAsync("The IP address has not changed; it's the same as before, so it wasn’t saved!");
            }
        }
        #endregion

        #region Create ViewModel 
        private AppParams parameters = AppParams.Instance;
        
        public HomePageVM(AppParams appParams)
        {
            UpdateProperties(appParams);

            UpdateIPProperties();

            askText     = "To use the application, go to the Settings page and configure the operating parameters.";
            onceText    = "The application is running in Once mode. It will check the IP address once and then wait for the user's command.\r\n" +
                          "You can switch to Loop mode in the Settings page if you want the application to continuously check the IP address at regular intervals.";
            loopText    = "The application is running in Loop mode. It will continuously check the IP address at regular intervals.\r\n\r\n" +
                          "This page is currently visible, but it can be minimized to a tray icon and will then run in the background.\r\n" + 
                          "If it detects a change to your public IP address, it will pop up in the foreground so you can see it. You can also trigger this by clicking the tray icon.\r\n" +
                          "If you click this text, the application will remain in the foreground until you close this window.\r\n" +
                          "This application will also automatically go to the background after 10 seconds if you do not make any changes on this screen.";
        }
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

        public void UpdateIPProperties(bool actualizeIP = true)
        {
            #region Set ViewModel IP variables 
            if (actualizeIP)
            {
                actIP    = Helper.GetPublicIP(parameters.WebURL);                                                           // This should be set to the actual current IP address
                lastRead = DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss");                                                    // This should be set to the actual last read time
            }

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
                    changedTextForeground = new SolidColorBrush(Colors.Green);
                    changedTextBackground = new SolidColorBrush(Colors.MintCream);
                }
                else
                {
                    changedText           = "IP Changed!";
                    changedTextForeground = new SolidColorBrush(Colors.Red);
                    changedTextBackground = new SolidColorBrush(Colors.Yellow);                     
                }
            }
            else
            {
                changedText = "No previous IP"; 
                changedTextForeground = new SolidColorBrush(Colors.Gray);
                changedTextBackground = new SolidColorBrush(Colors.LightGray);
            }

            ipHistory = new ObservableCollection<IPHistoryStruct>(tempList);

            #endregion
        }

        #region Tray Ikon

        public bool autoMinimizeToTrayIkon { get; set; } = false;                                                               // This property indicates whether the application should minimize to the system tray icon instead of closing completely

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)                                               // Override the OnPropertyChanged method to check any property changes for send/recall to/from Tray Ikon 
        {
            base.OnPropertyChanged(e);

            autoMinimizeToTrayIkon = false;                           

            Debug.WriteLine($"Property changed: {e.PropertyName}");
        }
        #endregion

        #region Helpers  
        public void RefreshView()
        {
            OnPropertyChanged(string.Empty);
        }    
        
        public async Task ShowMessageAsync(string message, int delayToClear = 2000)
        {
            this.message = message;

            await Task.Delay(delayToClear);

            this.message = string.Empty;
        }
        #endregion
    }    
}

