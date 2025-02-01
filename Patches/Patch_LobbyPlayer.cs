using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.Networkusername), MethodType.Setter)]
        private static void Networkusername_set_Prefix(ref string __0)
        {
            // Sanitize Username
            __0 = __0.RemoveRichText();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamUsername), MethodType.Setter)]
        private static void NetworksteamUsername_set_Prefix(ref string __0)
        {
            // Sanitize Username
            __0 = __0.RemoveRichText();
        }

#if RELEASE
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamID), MethodType.Setter)]
        private static void NetworksteamID_Prefix(LobbyPlayer __instance, ulong __0)
        {
            // Check for Host
            if (!NetworkServer.activeHost
                || (__instance == null)
                || __instance.WasCollected
                || (__instance.connectionToClient == null)
                || __instance.connectionToClient.WasCollected
                || !__instance.connectionToClient.isReady
                || !__instance.connectionToClient.isAuthenticated)
                return;

            // Player Check
            if (LobbySecurity.IsSteamIDInUse(__0))
            {
                __instance.connectionToClient.Disconnect();
                return;
            }

            // Add SteamID
            LobbySecurity.RemoveValidSteamID(__instance.steamID);
            LobbySecurity.AddValidSteamID(__0);
        }
#endif

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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Validate Server
            if ((__2.identity == null)
                || __2.identity.WasCollected
                || !LobbyManager.instance.gameStarted)
                return false;

            // Validate Sender
            LobbyPlayer sender = __0.TryCast<LobbyPlayer>();
            if ((sender == null)
                || sender.WasCollected
                || ((sender.NetworkplayerController != null)
                    && !sender.NetworkplayerController.WasCollected))
                return false;

            // Run Game Command
            sender.UserCode_CmdReplacePlayer__NetworkConnectionToClient(__2);

            // Prevent Original
            return false;
        }
    }
}
