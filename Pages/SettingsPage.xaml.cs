using AlertChangeIP.Classes;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;



namespace AlertChangeIP.Pages
{
    public sealed partial class SettingsPage : Page
    {
        internal AppParamsVM ViewModel { get; }

        public SettingsPage()
        {
            InitializeComponent();

            this.ViewModel   = new AppParamsVM(AppParams.Instance);
            this.DataContext = this.ViewModel;
        }
    }

    //

    #region Converters
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool b = value is bool boolean && boolean;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (value is Visibility v && v == Visibility.Visible);
        }
    }
    #endregion

    #region New LiveTextBox item for WinUI 3
    public class LiveTextBox : TextBox
    {   // Minden karakter módosítás után hívja a ViewModel változtatást, nem csak a szerkesztő mezőből kilépéskor
        public LiveTextBox()
        {
            this.TextChanged += (s, e) =>
            {
                this.GetBindingExpression(TextProperty)?.UpdateSource();
            };
        }
    }

    //<local:LiveTextBox Text = "{x:Bind ViewModel.TextValue, Mode=TwoWay}" />
    #endregion

}

// Install-Package CommunityToolkit.WinUI
// To learn more about WinUI, the WinUI project structure, and more about our project templates, see: http://aka.ms/winui-project-info.

