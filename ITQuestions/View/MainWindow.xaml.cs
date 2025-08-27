using ITQuestions.DB;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ITQuestions
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += MainWindow_Closing;

            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            if (!string.IsNullOrEmpty(version) && version.Contains('+'))
            {
                version = version.Split('+')[0];
            }

            VersionLabel.Content = $"App version: v{version}";
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            try
            {
                // run sync before app closes
                await SyncLocalAndRemote.Instance.SyncAsync();
            }
            catch (Exception ex)
            {
                // optional logging
                Console.WriteLine($"Sync failed: {ex.Message}");
            }
        }
    }
}