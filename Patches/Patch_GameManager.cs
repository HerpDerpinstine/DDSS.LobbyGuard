using HarmonyLib;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_GameManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ServerFirePlayer))]
        private static bool ServerFirePlayer_Prefix(NetworkIdentity __0)
        {
            // Check for Server
            if (__0.isServer)
                return true;

            // Validate Manager Role
            LobbyPlayer player = __0.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ServerSetAssistant))]
        private static bool ServerSetAssistant_Prefix(NetworkIdentity __0)
        {
            // Check for Server
            if (__0.isServer)
                return true;

            // Validate Manager Role
            LobbyPlayer player = __0.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }
    }
}