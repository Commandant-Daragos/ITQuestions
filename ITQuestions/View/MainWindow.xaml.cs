using ITQuestions.DB;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

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

            var exePath = Assembly.GetExecutingAssembly().Location;
            var info = FileVersionInfo.GetVersionInfo(exePath);
            var version = info.ProductVersion;

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
                await SyncLocalAndRemote.Instance.SyncAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Sync failed: {ex.Message}");
            }
        }
    }
}