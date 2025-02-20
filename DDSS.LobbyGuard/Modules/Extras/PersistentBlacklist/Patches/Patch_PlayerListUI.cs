using DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Internal;
using HarmonyLib;
using Il2Cpp;
using Il2CppPlayer.Lobby;
using Il2CppUI.Tabs.PlayerManagementTab;

namespace DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PlayerListUI
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerListUI), nameof(PlayerListUI.KickPlayer))]
        private static bool KickPlayer_Prefix(PlayerListUI __instance)
        {
            // Check for Server
            if (LobbyManager.instance == null
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if (player == null
                || player.WasCollected
                || player.isLocalPlayer)
                return false;

            // Kick Player
            BlacklistSecurity.RequestKick(player);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerListUI), nameof(PlayerListUI.BlacklistPlayer))]
        private static bool BlacklistPlayer_Prefix(PlayerListUI __instance)
        {
            // Check for Server
            if (LobbyManager.instance == null
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if (player == null
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