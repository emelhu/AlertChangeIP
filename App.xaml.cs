using AlertChangeIP.Classes;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System.Diagnostics;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AlertChangeIP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            string[] parameters = Environment.GetCommandLineArgs();

            AppParams.AppMode appMode = AppParams.AppMode.Ask;                                                                                  // Alapértelmezett érték

            if (parameters.Length >= 2)                                                                                                         // Első elem mindig az exe neve → a tényleges paraméter a [1]-ben van
            {
                string param1 = parameters[1].ToUpperInvariant();

                if (Enum.TryParse(param1, ignoreCase: true, out appMode))
                {
                    LoggingService.WriteLog($"Parsed AppMode parameter value: {appMode}");                                                                              
                }
                else
                {
                    LoggingService.WriteLog($"Invalid AppMode paramerter value: {param1}", LoggingService.LogLevel.Error);
                }
            }

            AppParams.appModeDefault = appMode;

            if (parameters.Length < 1)                                                                                                          // Elvileg ilyen nem lehetséges, de ha mégis, akkor logoljuk az esetet
            {
                LoggingService.WriteLog("Command line arguments should contain at least the executable name.", LoggingService.LogLevel.Error);
            }
            else
            { 
                LoggingService.WriteLog($"{parameters[0]} started with AppMode value: {appMode}");
            }
            //

            _window = new MainWindow();
            _window.Activate();
        }
    }
}
