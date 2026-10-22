using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AlertChangeIP.Pages
{
    public sealed partial class AboutPage : Page
    {
        public string aboutText {get; private set;} 

        public AboutPage()
        {
            InitializeComponent();

            aboutText = "This application is designed to help the user detect when their public IP address changes.\r\n\r\n" +
                        "When this happens, the application —which runs in the background— comes to the foreground and displays the change, allowing the user to take appropriate action.";

        }
    }
}
