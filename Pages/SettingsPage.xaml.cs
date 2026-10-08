using AlertChangeIP.Classes;

using Microsoft.UI.Xaml.Controls;



namespace AlertChangeIP.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        //

        public IList<AppParams.AppMode> appModeEnumsList { get; } = Enum.GetValues(typeof(AppParams.AppMode)).Cast<AppParams.AppMode>().ToList();             
    }
}

// Install-Package CommunityToolkit.WinUI
// To learn more about WinUI, the WinUI project structure, and more about our project templates, see: http://aka.ms/winui-project-info.

