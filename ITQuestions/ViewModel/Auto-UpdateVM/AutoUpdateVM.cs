using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.Model;
using ITQuestions.Service;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ITQuestions.ViewModel.Auto_UpdateVM;

public partial class AutoUpdateVM : ObservableObject
{
    private readonly UpdateService _updates = UpdateService.Instance;

    private string _currentVersion = "";
    public string CurrentVersion
    {
        get => _currentVersion;
        set { _currentVersion = value; OnPropertyChanged(); }
    }

    private string _latestVersion = "";
    public string LatestVersion
    {
        get => _latestVersion;
        set { _latestVersion = value; OnPropertyChanged(); }
    }

    private string _updateMessage = "";
    public string UpdateMessage
    {
        get => _updateMessage;
        set { _updateMessage = value; OnPropertyChanged(); }
    }

    private string? _downloadUrl;

    // --- new progress state ---
    private bool _isUpdating;
    public bool IsUpdating
    {
        get => _isUpdating;
        set
        {
            _isUpdating = value; 
            OnPropertyChanged();
            (UpdateCommand as IRelayCommand)?.NotifyCanExecuteChanged();
        }
    }

    private double _progressPercent;
    public double ProgressPercent
    {
        get => _progressPercent;
        set { _progressPercent = value; OnPropertyChanged(); }
    }

    private string _status = "";
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private CancellationTokenSource? _cts;

    public AutoUpdateVM() { }

    public void LoadFromInfo(UpdateInfo info, string currentVersion)
    {
        CurrentVersion = currentVersion;
        LatestVersion = info.LatestVersion ?? "";
        UpdateMessage = info.Changelog ?? "";
        _downloadUrl = info.DownloadUrl ?? "";
    }

    public bool CanUpdate => !IsUpdating && !string.IsNullOrWhiteSpace(_downloadUrl);

    [RelayCommand(CanExecute = nameof(CanUpdate))]
    private async Task Update()
    {
        if (string.IsNullOrWhiteSpace(_downloadUrl)) return;

        try
        {
            IsUpdating = true;
            Status = "Downloading update...";
            ProgressPercent = 0;
            _cts = new CancellationTokenSource();

            var progress = new Progress<double>(p => ProgressPercent = p);
            var tempExe = await _updates.DownloadNewExeAsync(_downloadUrl, progress, _cts.Token);

            Status = "Applying update...";
            await Task.Delay(200);

            _updates.LaunchSelfReplaceAndShutdown(tempExe);
            // app will exit here
        }
        catch (OperationCanceledException)
        {
            Status = "Update cancelled.";
        }
        catch (Exception ex)
        {
            Status = "Download failed. Opening release page...";
            try { Process.Start(new ProcessStartInfo(_downloadUrl!) { UseShellExecute = true }); } catch { }
        }
        finally
        {
            IsUpdating = false; // not reached if app exits successfully
        }
    }

    [RelayCommand]
    private void OpenDownloadPage()
    {
        if (!string.IsNullOrWhiteSpace(_downloadUrl))
        {
            try { Process.Start(new ProcessStartInfo(_downloadUrl) { UseShellExecute = true }); }
            catch { }
        }
    }
}