using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.JoinLogs.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamID), MethodType.Setter)]
        private static void NetworksteamID_Prefix(LobbyPlayer __instance, ulong __0)
        {
            // Check for Host
            if (!NetworkServer.activeHost
                || __instance == null
                || __instance.WasCollected
                || __instance.connectionToClient == null
                || __instance.connectionToClient.WasCollected
                || !__instance.connectionToClient.isReady
                || !__instance.connectionToClient.isAuthenticated
                || (__instance.steamID != 0))
                return;

            MelonMain._logger.Msg($"Player Joined: {__instance.NetworksteamUsername} - {__instance.Networkusername} - {__0}");
        }
    }
}
