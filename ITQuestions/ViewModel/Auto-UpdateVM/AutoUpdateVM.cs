using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITQuestions.Model;
using ITQuestions.Service;
using System.Diagnostics;

namespace ITQuestions.ViewModel.Auto_UpdateVM;

public partial class AutoUpdateVM : ObservableObject
{
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

    public AutoUpdateVM() { }

    public void LoadFromInfo(UpdateInfo info, string currentVersion)
    {
        CurrentVersion = currentVersion;
        LatestVersion = info.LatestVersion ?? "";
        UpdateMessage = info.Changelog ?? "";
        _downloadUrl = info.DownloadUrl ?? "";
    }

    [RelayCommand]
    private void OpenDownloadPage()
    {
        if (!string.IsNullOrWhiteSpace(_downloadUrl))
        {
            try { Process.Start(new ProcessStartInfo(_downloadUrl) { UseShellExecute = true }); }
            catch { /* log */ }
        }
    }

    [RelayCommand]
    private void Update()
    {
        OpenDownloadPage();
        // later: _updates.DownloadAndRunInstallerAsync(_downloadUrl);
    }
}