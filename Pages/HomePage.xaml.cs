using AlertChangeIP.Classes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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

        [ObservableProperty] public partial string actIP    { get; set; }
        [ObservableProperty] public partial string lastRead { get; set; }
        [ObservableProperty] public partial string prevIP   { get; set; }
        [ObservableProperty] public partial string prevRead { get; set; }
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

        #pragma warning disable CS8618
        public HomePageVM(AppParams appParams)
        {
            UpdateProperties(appParams);

            askText     = "To use the application, go to the Settings page and configure the operating parameters.";
            onceText    = "The application is running in Once mode. It will check the IP address once and then wait for the user's command.";
            loopText    = "The application is running in Loop mode. It will continuously check the IP address at regular intervals.";
        }
        #pragma warning restore CS8618
        #endregion

        public void UpdateProperties(AppParams appParams)
        {
            parameters = appParams;

            #region ViewModel variables initialize
            showOnce = (parameters.AppMode == AppParams.AppModeEnum.Once) ? Visibility.Visible : Visibility.Collapsed;
            showLoop = (parameters.AppMode == AppParams.AppModeEnum.Loop) ? Visibility.Visible : Visibility.Collapsed;
            showAsk  = (parameters.AppMode == AppParams.AppModeEnum.Ask)  ? Visibility.Visible : Visibility.Collapsed;
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

