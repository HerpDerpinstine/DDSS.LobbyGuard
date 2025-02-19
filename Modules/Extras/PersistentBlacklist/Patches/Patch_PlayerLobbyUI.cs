using HarmonyLib;
using Il2Cpp;
using Il2CppPlayer.Lobby;
using Il2CppUI.Tabs.LobbyTab;

namespace DDSS_LobbyGuard.PersistentBlacklist.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerLobbyUI
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.KickPlayer))]
        private static bool KickPlayer_Prefix(PlayerLobbyUI __instance)
        {
            // Check for Server
            if ((LobbyManager.instance == null)
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if ((player == null)
                || player.WasCollected
                || player.isLocalPlayer)
                return false;

            // Kick Player
            BlacklistSecurity.RequestKick(player);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.BlacklistPlayer))]
        private static bool BlacklistPlayer_Prefix(PlayerLobbyUI __instance)
        {
            // Check for Server
            if ((LobbyManager.instance == null)
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if ((player == null)
                || player.WasCollected
                || player.isLocalPlayer)
                return false;

            // Blacklist Player
            BlacklistSecurity.RequestBlacklist(player);

            // Prevent Original
            return false;
        }
    }
}