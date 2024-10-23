using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDSS_LobbyGuard.Security
{
    [Serializable]
    public class BlacklistedPlayer
    {
        public ulong SteamID;
        public string SteamName;
    }

    internal static class BlacklistSecurity
    {
        private static string _filePath;
        private static List<BlacklistedPlayer> _blacklist = new List<BlacklistedPlayer>();

        internal static void Setup()
        {
            _filePath = Path.Combine(MelonMain._userDataPath, "Blacklist.json");
            LoadFile();
            SaveFile();
        }

        internal static void OnLobbyOpen(LobbyManager manager)
        {
            if (!ConfigHandler._prefs_PersistentBlacklist.Value)
                return;
            manager.blacklist.Clear();

            LoadFile();

            if (_blacklist.Count > 0)
                foreach (BlacklistedPlayer player in _blacklist)
                    manager.blacklist.Add(player.SteamID);
        }

        internal static void OnBlacklistPlayer(ulong steamId, string steamName)
        {
            if (!ConfigHandler._prefs_PersistentBlacklist.Value)
                return;

            _blacklist.Add(new()
            {
                SteamID = steamId,
                SteamName = steamName
            });
            SaveFile();
            MelonMain._logger.Msg($"Blacklisted Player: {steamId} - {steamName}");
        }

        internal static void SaveFile()
            => File.WriteAllText(_filePath, JsonConvert.SerializeObject(_blacklist, Formatting.Indented));
        private static void LoadFile()
        {
            if (!File.Exists(_filePath))
                return;

            _blacklist = JsonConvert.DeserializeObject<List<BlacklistedPlayer>>(File.ReadAllText(_filePath));
        }
    }
}
