using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Fixes.NoVerifyError.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.VerifySteamId))]
        private static bool VerifySteamId_Prefix(LobbyPlayer __instance)
        {
            // Check for Host
            if (!NetworkServer.activeHost
                || __instance == null
                || __instance.WasCollected
                || __instance.connectionToClient == null
                || __instance.connectionToClient.WasCollected
                || !__instance.connectionToClient.isReady
                || !__instance.connectionToClient.isAuthenticated)
                return false;

            return true;
        }
    }
}
