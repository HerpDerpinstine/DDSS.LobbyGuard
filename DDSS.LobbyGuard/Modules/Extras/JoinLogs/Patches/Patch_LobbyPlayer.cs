using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.JoinLogs.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.VerifySteamId))]
        private static void VerifySteamId_Postfix(LobbyPlayer __instance, ulong __0, ulong __1)
        {
            // Check for Host
            if (!NetworkServer.activeHost
                || __instance == null
                || __instance.WasCollected
                || __instance.connectionToClient == null
                || __instance.connectionToClient.WasCollected
                || !__instance.connectionToClient.isReady
                || !__instance.connectionToClient.isAuthenticated
                || (__0 != 0))
                return;

            // Remove Steam ID
            LobbyPlayer localPlayer = LobbyManager.instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer != __instance)
                && (__1 != localPlayer.NetworksteamID))
            {
                MelonMain._logger.Msg($"Player Joined: {__instance.NetworksteamUsername} - {__instance.Networkusername} - {__1}");
            }
        }
    }
}
