using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using System;

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
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Prefix(LobbyPlayer __instance, NetworkReader __0, bool __1)
        {
            if (__instance._Mirror_SyncVarHookDelegate_steamID == null)
                __instance._Mirror_SyncVarHookDelegate_steamID = new Action<ulong, ulong>((ulong a, ulong b) => { });

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
        private static void SerializeSyncVars_Prefix(LobbyPlayer __instance, NetworkWriter __0, bool __1)
        {
            if (__instance._Mirror_SyncVarHookDelegate_steamID == null)
                __instance._Mirror_SyncVarHookDelegate_steamID = new Action<ulong, ulong>((ulong a, ulong b) => { });

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
                || !__instance.isServer
                || (__0 != PlayerRole.Slacker))
                return true;

            // Send Real Role to Player
            InteractionSecurity.AddSlacker(__instance);
            __instance.RpcSetPlayerRoleSpecific(__instance.connectionToClient, __0, true);

            // Check to Fake for All
            //if (!GameManager.instance.slackersCanSeeSlackers)
            //{
                // Send Fake Role to All
                foreach (NetworkConnectionToClient networkConnectionToClient in __instance.netIdentity.observers.Values)
                    if ((networkConnectionToClient != null)
                        && !networkConnectionToClient.WasCollected
                        && (networkConnectionToClient.connectionId != __instance.connectionToClient.connectionId))
                        __instance.RpcSetPlayerRoleSpecific(networkConnectionToClient, PlayerRole.Specialist, false);
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
            //                __instance.RpcSetPlayerRoleSpecific(networkConnectionToClient, PlayerRole.Slacker, false);

                            // Send player Role -> __instance
            //                player.RpcSetPlayerRoleSpecific(__instance.connectionToClient, PlayerRole.Slacker, false);
            //            }
            //            else
            //            {
                            // Send __instance Fake Role -> player
            //                __instance.RpcSetPlayerRoleSpecific(networkConnectionToClient, PlayerRole.Specialist, false);
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
