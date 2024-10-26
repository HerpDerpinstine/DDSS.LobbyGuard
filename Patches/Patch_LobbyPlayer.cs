using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdSetLocalPlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetLocalPlayer__NetworkConnectionToClient_Prefix(NetworkConnectionToClient __2)
        {
            // Validate Server
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient_Prefix(NetworkConnectionToClient __2)
        {
            // Validate Server
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdReplacePlayerWithSpectator__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdReplacePlayerWithSpectator__NetworkConnectionToClient_Prefix(NetworkConnectionToClient __2)
        {
            // Validate Server
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdSetSubRole__SubRole))]
        private static bool InvokeUserCode_CmdSetSubRole__SubRole_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get LobbyPlayer
            LobbyPlayer targetPlayer = __0.TryCast<LobbyPlayer>();
            if ((targetPlayer == null)
                || (targetPlayer.playerRole == PlayerRole.Manager))
                return false;

            // Validate Manager Role
            LobbyPlayer sender = __2.identity.GetComponent<LobbyPlayer>();
            if ((sender == null)
                || (sender.playerRole != PlayerRole.Manager)
                || (sender == targetPlayer))
                return false;

            // Validate Role
            SubRole requestedRole = (SubRole)__1.ReadInt();
            if (requestedRole == targetPlayer.NetworksubRole)
                return false;

            // Remove Assistant Role from All Others
            if (requestedRole == SubRole.Assistant)
            {
                foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedLobbyPlayers)
                {
                    // Get Old Player
                    LobbyPlayer oldPlayer = networkIdentity.GetComponent<LobbyPlayer>();
                    if ((oldPlayer == null)
                        || (oldPlayer.NetworksubRole != SubRole.Assistant))
                        continue;

                    // Reset Role
                    oldPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);
                }
            }

            // Run Game Command
            targetPlayer.UserCode_CmdSetSubRole__SubRole(requestedRole);

            // Prevent Original
            return false;
        }
    }
}
