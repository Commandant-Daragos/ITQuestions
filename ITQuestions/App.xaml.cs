using ITQuestions.DB;
using ITQuestions.Service;
using ITQuestions.View.Auto_Update;
using ITQuestions.View.SyncOnAppExit;
using ITQuestions.ViewModel.Auto_UpdateVM;
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
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await Current.Dispatcher.InvokeAsync(() =>
            {
                if (Current.MainWindow is MainWindow main)
                {
                    // Attach only once
                    main.Closing -= MainWindow_Closing;
                    main.Closing += MainWindow_Closing;
                }
            });

            // Start update check in background
            var currentVersion = UpdateService.ReadCurrentVersion();
            var (isUpdateAvailable, info) = await UpdateService.Instance.CheckForUpdatesAsync(currentVersion);

            if (isUpdateAvailable && info != null)
            {
                // show update window modally on top
                var vm = new AutoUpdateVM();
                vm.LoadFromInfo(info, currentVersion);

                var updateWin = new AutoUpdate()
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = vm
                };

                updateWin.ShowDialog();
                // user must close update window to continue using main window
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
                await SyncLocalAndRemote.Instance.SyncAsync();
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
