using ITQuestions.Model;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Windows;

namespace ITQuestions.Service
{
    public sealed class UpdateService
    {
        private static readonly Lazy<UpdateService> _instance = new(() => new UpdateService());
        public static UpdateService Instance => _instance.Value;
        private UpdateService() { }

        public async Task<UpdateInfo?> GetUpdateInfoAsync()
        {
            using var client = new HttpClient();
            var json = await client.GetStringAsync("https://github.com/Commandant-Daragos/ITQuestions/releases/latest/download/update.json");
            return JsonSerializer.Deserialize<UpdateInfo>(json);
        }

        public async Task<(bool isUpdateAvailable, UpdateInfo? Info)> CheckForUpdatesAsync(string currentVersion)
        {
            var info = await GetUpdateInfoAsync();
            bool isAvailable = false;
            if (info != null && !string.IsNullOrWhiteSpace(info.LatestVersion))
                isAvailable = CompareVersions(info.LatestVersion, currentVersion) > 0;
            return (isAvailable, info);
        }

        public static int CompareVersions(string a, string b)
        {
            string Clean(string v) => v.Split('+', '-')[0].TrimStart('v', 'V');
            if (Version.TryParse(Clean(a), out var va) && Version.TryParse(Clean(b), out var vb))
                return va.CompareTo(vb);
            return 0;
        }

        public static string ReadCurrentVersion()
        {
            var attr = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (!string.IsNullOrWhiteSpace(attr))
            {
                var plus = attr.IndexOf('+');
                return plus > 0 ? attr[..plus] : attr;
            }

            var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            return fvi.ProductVersion ?? fvi.FileVersion ??
                   Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        }

        public async Task<string> DownloadNewExeAsync(string downloadUrl, IProgress<double>? progress = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(downloadUrl))
                throw new ArgumentException("Download URL is empty", nameof(downloadUrl));

            var tempPath = Path.Combine(Path.GetTempPath(), "ITQuestions_Update.exe");

            using var client = new HttpClient();
            using var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            var total = response.Content.Headers.ContentLength ?? -1L;
            var canReport = total > 0 && progress != null;

            await using var httpStream = await response.Content.ReadAsStreamAsync(ct);
            await using var fileStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);

            var buffer = new byte[81920];
            long readTotal = 0;
            int read;
            while ((read = await httpStream.ReadAsync(buffer.AsMemory(0, buffer.Length), ct)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, read), ct);
                readTotal += read;
                if (canReport)
                {
                    var pct = (double)readTotal / total * 100.0;
                    progress!.Report(pct);
                }
            }

            return tempPath; // path to the downloaded exe in %TEMP%
        }

        public void LaunchSelfReplaceAndShutdown(string tempExePath)
        {
            if (string.IsNullOrWhiteSpace(tempExePath) || !File.Exists(tempExePath))
                throw new FileNotFoundException("Downloaded updater exe not found.", tempExePath);

            var targetExe = Process.GetCurrentProcess().MainModule!.FileName!;

            Process.Start(new ProcessStartInfo
            {
                FileName = tempExePath,
                Arguments = $"--replace \"{targetExe}\"",
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
    }
}
