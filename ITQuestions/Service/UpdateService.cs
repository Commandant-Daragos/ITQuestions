using ITQuestions.Model;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

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
    }
}
