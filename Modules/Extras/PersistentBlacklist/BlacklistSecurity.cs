using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace DDSS_LobbyGuard.PersistentBlacklist
{
    internal static class BlacklistSecurity
    {
        internal static bool _error;
        internal static bool _errorSave;
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
            if (_error)
                return;

            LoadFile();

            if (!_error && (_blacklist.Count > 0))
                foreach (BlacklistedPlayer player in _blacklist)
                    if ((player.SteamID != 0)
                        && !manager.blacklist.Contains(player.SteamID))
                        manager.blacklist.Add(player.SteamID);
        }

        internal static void SaveFile()
        {
            if (_error)
                return;

            string fileContent = JsonConvert.SerializeObject(_blacklist, Formatting.Indented);
            if (string.IsNullOrEmpty(fileContent)
                || string.IsNullOrWhiteSpace(fileContent))
                return;

            try
            {
                File.WriteAllText(_filePath, fileContent);
            }
            catch (Exception ex)
            {
                _error = true;
                _errorSave = true;
                MelonMain._logger.Error($"Failed to Save Blacklist.json\n{ex}");
            }
        }
        private static void LoadFile()
        {
            if (!File.Exists(_filePath))
                return;

            string fileContent = File.ReadAllText(_filePath);
            if (string.IsNullOrEmpty(fileContent)
                || string.IsNullOrWhiteSpace(fileContent))
                return;

            try
            {
                _blacklist = JsonConvert.DeserializeObject<List<BlacklistedPlayer>>(fileContent);
            }
            catch (Exception ex)
            {
                _error = true;
                _errorSave = false;
                MelonMain._logger.Error($"Failed to Load Blacklist.json\n{ex}");
            }
        }

        internal static void RequestKick(LobbyPlayer player)
        {
            //if (ConfigHandler.Moderation.PromptForKick.Value)
            //    MelonMain.ShowPrompt("Moderation Confirmation", $"Kick {player.steamUsername}", "Confirm", "Cancel", new Action(() => ApplyKick(player)), new Action(() => { }));
            //else
                ApplyKick(player);
        }

        internal static void RequestBlacklist(LobbyPlayer player)
        {
            //if (ConfigHandler.Moderation.PromptForBlacklist.Value)
            //    MelonMain.ShowPrompt("Moderation Confirmation", $"Blacklist {player.steamUsername}", "Confirm", "Cancel", new Action(() => ApplyBlacklist(player)), new Action(() => { }));
            //else
                ApplyBlacklist(player);
        }

        private static void OnBlacklistPlayer(ulong steamId, string name)
        {
            if (_error)
                return;

            _blacklist.Add(new()
            {
                Timestamp = DateTime.Now.ToUniversalTime().ToString("G", DateTimeFormatInfo.InvariantInfo),
                SteamID = steamId,
                Name = name
            });
            SaveFile();
        }

        private static void ApplyKick(LobbyPlayer player, bool isBlacklist = false)
        {
            // Force-Disconnect
            if ((player != null)
                && !player.WasCollected
                && (player.connectionToClient != null)
                && !player.connectionToClient.WasCollected
                && (player.connectionToClient is not LocalConnectionToClient))
            {
                if (!isBlacklist)
                    MelonMain._logger.Msg($"Kicked Player: {player.NetworksteamUsername} - {player.Networkusername} - {player.NetworksteamID}");
                player.connectionToClient.Disconnect();
            }
        }

        private static void ApplyBlacklist(LobbyPlayer player)
        {
            if ((player != null)
                && !player.WasCollected
                && (player.connectionToClient != null)
                && !player.connectionToClient.WasCollected
                && (player.connectionToClient is not LocalConnectionToClient))
            {
                OnBlacklistPlayer(player.NetworksteamID, player.NetworksteamUsername);
                LobbyManager.instance.blacklist.Add(player.NetworksteamID);
                MelonMain._logger.Msg($"Blacklisted Player: {player.NetworksteamUsername} - {player.Networkusername} - {player.NetworksteamID}");
            }
        }
    }
}