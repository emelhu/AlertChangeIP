using AlertChangeIP.Classes;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;



namespace AlertChangeIP.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        AppParams appParams = new();

        //

        public IList<AppParams.AppMode> appModeEnumsList { get; } = Enum.GetValues(typeof(AppParams.AppMode)).Cast<AppParams.AppMode>().ToList();

        public async void SetDefaultWebURL(object sender, RoutedEventArgs e)
        {
            await FocusManager.TryFocusAsync(urlListCheckBox, FocusState.Programmatic);

            //appParams.webURL = AppParams.defaultWebURL;                                                                                     // old try: this.Bindings.Update();

            appParams.webURLChange(AppParams.defaultWebURL);            
        }

        private bool showURLList_ = false;
        public bool showURLList { get { return showURLList_; } set { showURLList_ = value; this.Bindings.Update(); } }
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

