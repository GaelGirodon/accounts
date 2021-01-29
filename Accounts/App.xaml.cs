using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Accounts.Windows;

namespace Accounts
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            // Set culture for WPF formats
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage
                    .GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            // Start the main window
            var window = new MainWindow();
            if (e.Args.Length == 1) // Open with
                window.OpenFile(e.Args[0]);
            window.Show();
        }
    }
}
