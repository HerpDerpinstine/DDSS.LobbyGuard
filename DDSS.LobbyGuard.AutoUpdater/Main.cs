using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DDSS_LobbyGuard
{
    public class MelonMain : MelonPlugin
    {
        private const string _url = "https://api.github.com/repos/HerpDerpinstine/DDSS.LobbyGuard/releases";
        private static HttpClient _client = new();

        public static string _userDataPath { get; private set; }
        public static MelonLogger.Instance _logger { get; private set; }

        public override void OnInitializeMelon()
        {
            // Cache Logger 
            _logger = LoggerInstance;

            // Setup UserData Folder
            _userDataPath = Path.Combine(MelonEnvironment.UserDataDirectory, Properties.BuildInfo.Name);
            if (!Directory.Exists(_userDataPath))
                Directory.CreateDirectory(_userDataPath);

            try
            {
                SemVersion currentVersion = SemVersion.Parse(Properties.BuildInfo.Version);
                (SemVersion, string) newestVersion = CheckAPI(currentVersion).Result;

                _logger.Msg($"Latest Version: {newestVersion}");

                if (newestVersion.Item1 <= currentVersion)
                    return;

                _logger.Msg("Update is Available!");

                if (string.IsNullOrEmpty(newestVersion.Item2)
                    || string.IsNullOrWhiteSpace(newestVersion.Item2))
                {

                    return;
                }

                // Download New Version

                // Delete Old Mod

                // Delete Old Modules

                // Extract New Version - Mod

                // Extract New Version - Modules

                // Log Success
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private static async Task<(SemVersion, string)> CheckAPI(SemVersion currentVersion)
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
                return (currentVersion, null);

            JArray data = (JArray)JsonConvert.DeserializeObject(response);

            SemVersion highestVersion = currentVersion;
            string downloadLink = null;
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
                {
                    highestVersion = semVersion;
                }
            }

            return (highestVersion, downloadLink);
        }
    }
}