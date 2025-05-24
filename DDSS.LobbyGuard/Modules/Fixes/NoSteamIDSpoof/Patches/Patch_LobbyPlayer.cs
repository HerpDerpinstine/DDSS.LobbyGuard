using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Fixes.NoSteamIDSpoof.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.VerifySteamId))]
        private static void VerifySteamId_Postfix(LobbyPlayer __instance, ulong __1)
        {
            // Check for Host
            if (!NetworkServer.activeHost
                || !ModuleMain.IsSteamLobby
                || __instance == null
                || __instance.WasCollected
                || __instance.connectionToClient == null
                || __instance.connectionToClient.WasCollected
                || !__instance.connectionToClient.isReady
                || !__instance.connectionToClient.isAuthenticated)
                return;

            // Verify the Steam ID has not been Spoofed
            LobbyPlayer localPlayer = LobbyManager.instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer != __instance))
            {
                if ((__1 == localPlayer.NetworksteamID)
                    || !ModuleMain.IsSteamIDValid(__1))
                    __instance.connectionToClient.Disconnect();
            }
        }
    }
}
