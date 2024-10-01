using HarmonyLib;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_GameManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdFirePlayer__NetworkIdentity__Boolean__Boolean))]
        private static bool InvokeUserCode_CmdFirePlayer__NetworkIdentity__Boolean__Boolean_Prefix(NetworkConnectionToClient __2)
        {
            // Validate Manager Role
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdSetAssistant__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSetAssistant__NetworkIdentity_Prefix(NetworkConnectionToClient __2)
        {
            // Validate Manager Role
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdSetState__Int32))]
        private static bool InvokeUserCode_CmdSetState__Int32_Prefix(NetworkConnectionToClient __2)
        {
            // Check for Host
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }
    }
}