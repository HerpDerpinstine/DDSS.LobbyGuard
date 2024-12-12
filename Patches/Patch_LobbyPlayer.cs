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

        //[HarmonyPrefix]
        //[HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamID), MethodType.Setter)]
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Prefix(LobbyPlayer __instance)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return;
            if (!NetworkServer.activeHost)
                return;
            if (__instance.NetworkisFired)
                return;
            if (GameManager.instance == null)
                return;
            if (InteractionSecurity.GetWinner(GameManager.instance) != PlayerRole.None)
                return;
            
            if (__instance.playerRole == PlayerRole.Slacker)
                __instance.NetworkplayerRole = PlayerRole.Specialist;
            if (__instance.originalPlayerRole == PlayerRole.Slacker)
                __instance.NetworkoriginalPlayerRole = PlayerRole.Specialist;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Prefix(LobbyPlayer __instance)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return;
            if (!NetworkServer.activeHost)
                return;
            if (GameManager.instance == null)
                return;
            if (__instance.NetworkisFired)
                return;
            if (InteractionSecurity.GetWinner(GameManager.instance) != PlayerRole.None)
                return;

            if (__instance.playerRole == PlayerRole.Slacker)
                __instance.NetworkplayerRole = PlayerRole.Specialist;
            if (__instance.originalPlayerRole == PlayerRole.Slacker)
                __instance.NetworkoriginalPlayerRole = PlayerRole.Specialist;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.RpcSetPlayerRole))]
        private static bool RpcSetPlayerRole_Prefix(LobbyPlayer __instance,
            ref PlayerRole __0,
            bool __1)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return true;

            // Check for Slacker Role
            if (!NetworkServer.activeHost
                || !__instance.isServer)
                return true;
            if (__0 != PlayerRole.Slacker)
            {
                InteractionSecurity.RemoveSlacker(__instance);
                return true;
            }

            // Send Real Role to Player
            InteractionSecurity.AddSlacker(__instance);
            __instance.CustomRpcSetPlayerRole(__0, true, __instance.connectionToClient);

            // Check to Fake for All
            //if (!GameManager.instance.slackersCanSeeSlackers)
            //{
                // Send Fake Role to All
                foreach (NetworkConnectionToClient networkConnectionToClient in __instance.netIdentity.observers.Values)
                    if ((networkConnectionToClient != null)
                        && !networkConnectionToClient.WasCollected
                        && (networkConnectionToClient.connectionId != __instance.connectionToClient.connectionId))
                        __instance.CustomRpcSetPlayerRole(PlayerRole.Specialist, false, networkConnectionToClient);
            //}
            //else
            //{
                // Iterate through All Observers
            //    foreach (NetworkConnectionToClient networkConnectionToClient in __instance.netIdentity.observers.Values)
            //        if ((networkConnectionToClient != null)
            //            && !networkConnectionToClient.WasCollected
            //            && (networkConnectionToClient.connectionId != __instance.connectionToClient.connectionId))
            //        {
            //            LobbyPlayer player = networkConnectionToClient.identity.GetComponent<LobbyPlayer>();
            //            if (InteractionSecurity.IsSlacker(player)
            //                || (player.NetworkplayerRole == PlayerRole.Slacker))
            //            {
                            // Send __instance Role -> player
            //                __instance.RpcSetPlayerRoleSpecific(PlayerRole.Slacker, false, networkConnectionToClient);

                            // Send player Role -> __instance
            //                player.RpcSetPlayerRoleSpecific(PlayerRole.Slacker, false, __instance.connectionToClient);
            //            }
            //            else
            //            {
                            // Send __instance Fake Role -> player
            //                __instance.RpcSetPlayerRoleSpecific(PlayerRole.Specialist, false, networkConnectionToClient);
            //            }
            //        }
            //}

            // Prevent Original
            return false;
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
                || (targetPlayer.NetworkplayerRole == PlayerRole.Manager))
                return false;

            // Validate Manager Role
            LobbyPlayer sender = __2.identity.GetComponent<LobbyPlayer>();
            if ((sender == null)
                || (sender.NetworkplayerRole != PlayerRole.Manager)
                || (sender == targetPlayer))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, targetPlayer.transform.position))
                return false;

            // Validate Role
            SubRole requestedRole = (SubRole)__1.SafeReadInt();
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
