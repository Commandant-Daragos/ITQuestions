using ITQuestions.DB;
using ITQuestions.View.SyncOnAppExit;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ITQuestions
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            var main = Application.Current.MainWindow;
            if (main != null)
            {
                // avoid duplicate event handlers if OnActivated fires multiple times
                main.Closing -= MainWindow_Closing;
                main.Closing += MainWindow_Closing;
            }
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // Cancel the close so we can finish sync
            e.Cancel = true;

            var syncingWindow = new SyncWindow(); // a simple WPF Window with spinner
            syncingWindow.Show();

            try
            {
                // run sync
                await SyncLocalAndRemote.Instance.PushLocalToRemoteAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to sync: {ex.Message}");
            }
            finally
            {
                syncingWindow.Close();
                // Now really close the app
                Application.Current.MainWindow.Closing -= MainWindow_Closing;
                Application.Current.Shutdown();
            }
        }
    }
}
