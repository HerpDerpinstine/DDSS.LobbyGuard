﻿using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DDSS_LobbyGuard.Security
{
    [Serializable]
    public class BlacklistedPlayer
    {
        public ulong SteamID;
        public string Name;
        public string Timestamp;
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

            LoadFile();

            if (_blacklist.Count > 0)
                foreach (BlacklistedPlayer player in _blacklist)
                    if (!manager.blacklist.Contains(player.SteamID))
                        manager.blacklist.Add(player.SteamID);
        }

        internal static void OnBlacklistPlayer(ulong steamId, string name)
        {
            if (!ConfigHandler._prefs_PersistentBlacklist.Value)
                return;

            _blacklist.Add(new()
            {
                SteamID = steamId,
                Name = name,
                Timestamp = DateTime.Now.ToUniversalTime().ToString("G", DateTimeFormatInfo.InvariantInfo),
            });
            SaveFile();
            MelonMain._logger.Msg($"Blacklisted Player: {steamId} - {name}");
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
