using ITQuestions.DB;
using ITQuestions.Service;
using ITQuestions.View.Auto_Update;
using ITQuestions.View.SyncOnAppExit;
using ITQuestions.ViewModel.Auto_UpdateVM;
using System.ComponentModel;
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
                    main.Closing -= MainWindow_Closing;
                    main.Closing += MainWindow_Closing;
                }
            });

            var currentVersion = UpdateService.ReadCurrentVersion();
            var (isUpdateAvailable, info) = await UpdateService.Instance.CheckForUpdatesAsync(currentVersion);
            if (isUpdateAvailable && info != null)
            {
                var vm = new AutoUpdateVM();
                vm.LoadFromInfo(info, currentVersion);

                var updateWin = new AutoUpdate()
                {
                    Owner = Current.MainWindow,
                    DataContext = vm
                };

                updateWin.ShowDialog();
            }
        }

        private async void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = true;

            var syncingWindow = new SyncWindow();
            syncingWindow.Show();

            try
            {
                await SyncLocalAndRemote.Instance.SyncAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to sync: {ex.Message}");
            }
            finally
            {
                syncingWindow.Close();
                Current.MainWindow.Closing -= MainWindow_Closing;
                Current.Shutdown();
            }
        }
    }
}
