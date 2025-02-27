using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateUsers.Patches
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
                || !__instance.connectionToClient.isAuthenticated)
                return;

            LobbyPlayer localPlayer = LobbyManager.instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer != __instance))
            {
                MelonMain._logger.Msg(__1);
                MelonMain._logger.Msg(localPlayer.NetworksteamID);

                // Player Check
                if ((__1 == localPlayer.NetworksteamID)
                    || ModuleMain.IsSteamIDInUse(__1))
                {
                    __instance.connectionToClient.Disconnect();
                    return;
                }

                // Add SteamID
                ModuleMain.RemoveValidSteamID(__0);
                ModuleMain.AddValidSteamID(__1);
            }
        }
    }
}
