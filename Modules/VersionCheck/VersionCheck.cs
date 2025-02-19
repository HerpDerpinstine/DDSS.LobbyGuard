using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace DDSS_LobbyGuard.VersionCheck
{
    internal static class VersionCheck
    {
        private const string _url = "https://api.github.com/repos/HerpDerpinstine/DDSS.LobbyGuard/releases";
        private static HttpClient _client = new();
        private static bool _hasChecked;

        public static void Run()
        {
            if (_hasChecked)
                return;
            _hasChecked = true;

            try
            {
                SemVersion currentVersion = SemVersion.Parse(Properties.BuildInfo.Version);
                SemVersion newestVersion = CheckAPI(currentVersion).Result;

                MelonMain._logger.Msg($"Latest Version: {newestVersion}");

                if (newestVersion <= currentVersion)
                    return;

                MelonMain._logger.Msg("Update is Available!");

                MelonMain.ShowPrompt("Update Available",
                    $"LobbyGuard v{newestVersion} is available for download!\nQuit and Open Download Page?",
                    "Play Game",
                    "Quit and Open Page",
                    null,
                    new Action(() =>
                    {
                        Application.OpenURL($"{Properties.BuildInfo.DownloadLink}/releases/latest");
                        Application.Quit();
                    }),
                    null);
            }
            catch (Exception ex)
            {
                MelonMain._logger.Error(ex);
            }
        }

        private static async Task<SemVersion> CheckAPI(SemVersion currentVersion)
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", $"DDSS.{Properties.BuildInfo.Name} v{Properties.BuildInfo.Version}");

            string response = null;
            try
            {
                response = await _client.GetStringAsync(_url);
            }
            catch
            {
                response = null;
            }

            if (string.IsNullOrEmpty(response))
                return currentVersion;

            JArray data = (JArray)JsonConvert.DeserializeObject(response);

            SemVersion highestVersion = currentVersion;
            foreach (var release in data)
            {
                JArray assets = (JArray)release["assets"];
                if (assets.Count <= 0)
                    continue;
                if ((bool)release["prerelease"])
                    continue;

                string version = (string)release["tag_name"];
                SemVersion semVersion = SemVersion.Parse(version.Substring(1));
                if (semVersion > highestVersion)
                    highestVersion = semVersion;
            }

            return highestVersion;
        }
    }
}
