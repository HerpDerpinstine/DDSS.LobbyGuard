using DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Internal;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Awake))]
        private static void Awake_Postfix(LobbyManager __instance)
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return;

            // Update Blacklist
            BlacklistSecurity.OnLobbyOpen(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.ServerKickPlayer))]
        private static bool ServerKickPlayer_Prefix(LobbyManager __instance,
            NetworkIdentity __0)
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Validate Player
            if (__0 == null
                || __0.WasCollected
                || __0.isLocalPlayer)
                return false;

            // Kick Player
            BlacklistSecurity.RequestKick(__0.GetComponent<LobbyPlayer>());

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.BlackListPlayer))]
        private static bool BlackListPlayer_Prefix(LobbyManager __instance,
            LobbyPlayer __0)
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Validate Player
            if (__0 == null
                || __0.WasCollected
                || __0.isLocalPlayer
                || __0.NetworksteamID == 0)
                return false;

            // Blacklist Player
            BlacklistSecurity.RequestBlacklist(__0);

            // Prevent Original
            return false;
        }
    }
}