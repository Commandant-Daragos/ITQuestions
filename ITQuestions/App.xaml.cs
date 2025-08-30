using ITQuestions.DB;
using ITQuestions.Service;
using ITQuestions.View.Auto_Update;
using ITQuestions.View.SyncOnAppExit;
using ITQuestions.ViewModel.Auto_UpdateVM;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

            // --- self-replace mode (runs from the temp exe) ---
            if (e.Args.Length >= 2 && e.Args[0] == "--replace")
            {
                string targetPath = e.Args[1];
                string me = Process.GetCurrentProcess().MainModule!.FileName!;

                try
                {
                    // wait for old process to exit (extra safety)
                    await Task.Delay(800);

                    // try a few times in case filesystem lag
                    const int maxTries = 20;
                    for (int i = 0; i < maxTries; i++)
                    {
                        try
                        {
                            File.Copy(me, targetPath, overwrite: true);
                            break;
                        }
                        catch
                        {
                            await Task.Delay(250);
                            if (i == maxTries - 1) throw;
                        }
                    }

                    // restart updated app
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = targetPath,
                        UseShellExecute = true
                    });

                    Current.Shutdown();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Update failed: {ex.Message}", "Updater", MessageBoxButton.OK, MessageBoxImage.Error);
                    // fall through to normal startup so user isn’t stranded
                }
            }

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
