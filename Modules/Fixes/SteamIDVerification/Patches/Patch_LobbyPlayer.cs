using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.SteamIDVerification.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.VerifySteamId))]
        private static bool VerifySteamId_Prefix()
        {
            // Check for Server
            if (!NetworkServer.activeHost)
                return false;

            // Run Original
            return true;
        }
    }
}
