using AlertChangeIP.Classes;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using System.Diagnostics;

using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

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

        private async void Test1_Click(object sender, RoutedEventArgs e)
        {   // Fill IP history with test data
            var panel = new StackPanel();

            panel.Children.Add(new TextBlock
            {
                Text = "Are you sure you want to export the IP history, thereby overwriting the valid data?",
                Margin = new Thickness(10)
            });

            panel.Children.Add(new TextBlock
            {
                Text = "For security reasons, please enter the word \"YES\" exactly in the input field below.\r\nThis confirms your decision.\r\nThis is to prevent errors.",
                Margin = new Thickness(0, 0, 0, 10)
            });

            var inputBox = new TextBox
            {
                PlaceholderText = "YES?"
            };

            panel.Children.Add(inputBox);

            var dialog = new ContentDialog
            {
                Title = "Confirmation",
                Content = panel,
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            { 
                string answer = inputBox.Text ?? "NO";
                if (answer.CompareTo("YES") == 0)
                {
                    IPHistoryStore.defaultReadedHistoryItemsCount = 100;

                    for (int loop = 0; loop < 200; loop++)
                    {
                        string ipText = $"1.2.3.{loop}";
                        IPHistoryStore.Write(ipText);
                        messageText.Text = $"Generated test IP: {ipText}";

                        await Task.Delay(100);
                    }

                    messageText.Text = "Test 1 done";
                }
                else
                {
                    messageText.Text = "Test 1: none YES text";
                }
            }
            else
            {
                messageText.Text = "Test 1: Cancelled";
            }
        }

        private void Test2_Click(object sender, RoutedEventArgs e)
        {
        }

        private void IPHistoryExport_Click(object sender, RoutedEventArgs e)
        {   // Exporting IP history to a text file
            string tempFile = Path.GetTempFileName() + ".txt";
            var    list     = IPHistoryStore.Read(0);                                                                       // 0: signals, use default count of items

            if (list is null)
            {
                System.IO.File.WriteAllText(tempFile, "!!! ERROR !!! Can't read IP history file!");
            }
            else
            { 
                list.Reverse();

                foreach (var item in list)
                {
                    string ip = Helper.IntToIp(item.ip);
                    string dt = item.when.ToString("yyyy.MM.dd hh.mm.ss");

                    System.IO.File.AppendAllText(tempFile, $"IP: {ip}  Time: {dt}\r\n");
                } 
            }    

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile, UseShellExecute = true
            });
        }
    }
}
