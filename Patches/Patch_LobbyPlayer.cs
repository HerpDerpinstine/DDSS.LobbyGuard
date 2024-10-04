using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_LobbyPlayer
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.Networkusername), MethodType.Getter)]
        private static void Networkusername_get_Postfix(ref string __result)
        {
            // Sanitize Username
            __result = __result.RemoveRichText();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.Networkusername), MethodType.Setter)]
        private static void Networkusername_set_Prefix(ref string __0)
        {
            // Sanitize Username
            __0 = __0.RemoveRichText();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamUsername), MethodType.Getter)]
        private static void NetworksteamUsername_get_Postfix(ref string __result)
        {
            // Sanitize Username
            __result = __result.RemoveRichText();
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
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Validate Manager Role
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Validate Role
            SubRole requestedRole = (SubRole)__1.ReadInt();
            if (requestedRole == player.subRole)
                return false;

            // Remove Assistant Role from All Others
            if (requestedRole == SubRole.Assistant)
            {
                foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedPlayers)
                {
                    // Get Old Player
                    LobbyPlayer oldPlayer = networkIdentity.GetComponent<LobbyPlayer>();
                    if ((oldPlayer == null)
                        || (oldPlayer.subRole != SubRole.Assistant))
                        continue;

                    // Reset Role
                    oldPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);
                }
            }

            // Run Game Command
            player.UserCode_CmdSetSubRole__SubRole(requestedRole);

            // Prevent Original
            return false;
        }
    }
}
