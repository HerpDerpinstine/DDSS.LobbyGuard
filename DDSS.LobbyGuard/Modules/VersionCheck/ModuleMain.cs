using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Semver;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.VersionCheck
{
    internal class ModuleMain : ILobbyModule
    {
        private const string _url = "https://api.github.com/repos/HerpDerpinstine/DDSS.LobbyGuard/releases";
        private HttpClient _client = new();
        private bool _hasChecked;

        public override string Name => "VersionCheck";
        public override int Priority => 100;
        public override Type ConfigType => typeof(ModuleConfig);

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;
            CheckForUpdates();
        }

        private void CheckForUpdates()
        {
            if (_hasChecked)
                return;
            _hasChecked = true;

            if (!ModuleConfig.Instance.CheckForUpdates.Value)
                return;

            try
            {
                SemVersion currentVersion = SemVersion.Parse(Properties.BuildInfo.Version);
                SemVersion newestVersion = CheckAPI(currentVersion).Result;

                MelonMain._logger.Msg($"Latest Version: {newestVersion}");

                if (newestVersion <= currentVersion)
                    return;

                MelonMain._logger.Msg("Update is Available!");


                if (!ModuleConfig.Instance.ShowPrompt.Value)
                    return;
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

        private async Task<SemVersion> CheckAPI(SemVersion currentVersion)
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