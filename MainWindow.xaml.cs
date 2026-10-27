using AlertChangeIP.Classes;
using AlertChangeIP.Pages;

using CommunityToolkit.Mvvm.Input;

using H.NotifyIcon;                                                                                                                             // NuGet package: Install-Package H.NotifyIcon.WinUI
using WinRT.Interop;                                                                                                                            // for WindowNative.GetWindowHandle(this) for H.NotifyIcon

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AlertChangeIP
{
    public sealed partial class MainWindow : Window
    {        
        public MainWindow()
        {            
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(AppTitleBar);
            AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;
            AppWindow.SetIcon("Assets/AppIcon.ico");

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);

            _appWindow = AppWindow.GetFromWindowId(windowId);

            this.Closed += OnClosed;
            ShowWindowCommand = new RelayCommand(RestoreFromTray);                                                                                   // for tray icon click event, H.NotifyIcon extension will keep the application running in the background

            WindowStateStorage.Restore(AppWindow);                                                                                              // Restore the window state when the window is opened   
        }

        private async void OnClosed(object sender, WindowEventArgs e)
        {
            WindowStateStorage.Save(AppWindow);                                                                                                 // Save the window state when the window is closed                                

            if (AppParams.Instance.handleWindowClose)
            {
                e.Handled = true;

                HideToTray();
            }
        }

        #region Tray Icon
        private AppWindow       _appWindow;
        public  IRelayCommand   ShowWindowCommand { get; }  

        private void RestoreFromTray()
        {
            _appWindow.Show();
            this.Activate();

            TrayIcon.Visibility = Visibility.Collapsed;
        }

        public void HideToTray()
        {
            TrayIcon.ForceCreate();
            TrayIcon.Visibility = Visibility.Visible;
            //this.Hide();                                                                                                                    // Hide the window instead of closing it; H.NotifyIcon extension will keep the application running in the background
            _appWindow.Hide();
        }

        private void TrayIcon_OpenClick(object sender, RoutedEventArgs e)
        {
            RestoreFromTray();
        }

        private void TrayIcon_ExitClick(object sender, RoutedEventArgs e)
        {
            AppParams.Instance.handleWindowClose = false;

            TrayIcon.Dispose();
            Application.Current.Exit();
        }
        #endregion

        #region Title Bar
        private void TitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            NavView.IsPaneOpen = !NavView.IsPaneOpen;
        }

        private void TitleBar_BackRequested(TitleBar sender, object args)
        {
            NavFrame.GoBack();
        }

        private async void TitleBar_CloseOrHideClick(object sender, RoutedEventArgs e)                                                            // Added by eMeL
        {
            bool answer = await Helper.ShowDialogAsync(((FrameworkElement)this.Content), "Which action would you like to take? 'Close' or 'Hide' the window?", "'Close' or 'Hide' the window?", "Hide", "Close");

            AppParams.Instance.handleWindowClose = answer;
        }
        #endregion

        #region Navigation 

        private static bool? toSettingsPage = null;
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            AppParams appParams = AppParams.Instance;

            if (toSettingsPage is null)
            {
                toSettingsPage = (appParams.AppMode == AppParams.AppModeEnum.Ask);
            }

            if (toSettingsPage ?? true)
            {
                toSettingsPage = false;
                NavFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.IsSettingsSelected)
            {
                NavFrame.Navigate(typeof(SettingsPage));
            }
            else if (args.SelectedItem is NavigationViewItem item)
            {
                switch (item.Tag)
                {
                    case "home":
                        NavFrame.Navigate(typeof(HomePage));
                        break;
                    case "about":
                        NavFrame.Navigate(typeof(AboutPage));
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown navigation item tag: {item.Tag}");
                }
            }
        }
        #endregion
    }
}
