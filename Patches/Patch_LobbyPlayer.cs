﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.WorkStation.Phone;

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
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.ServerSetWorkStation))]
        private static void ServerSetWorkStation_Prefix(LobbyPlayer __instance, WorkStationController __0)
        {
            if ((__0 != null)
                && !__0.WasCollected)
            {
                PhoneController phone = __0.phoneController;
                if ((phone != null)
                    && !phone.WasCollected)
                    phone.ForceCallToEnd();
            }

            if ((__instance.NetworkworkStationController != null)
                && !__instance.NetworkworkStationController.WasCollected)
            {
                PhoneController phone = __instance.NetworkworkStationController.phoneController;
                if ((phone != null)
                    && !phone.WasCollected)
                    phone.ForceCallToEnd();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Prefix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Postfix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Prefix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Postfix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);

        private static void EnforcePlayerValues(LobbyPlayer __instance)
        {
            if (!NetworkServer.activeHost
                || (GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return;

            // Check if Win Screen is Hidden
            if (InteractionSecurity.GetWinner(GameManager.instance) == PlayerRole.None)
            {
                // Force NetworkisFired to false for Janitors to be Reassignable
                if (ConfigHandler.Gameplay.AllowJanitorsToKeepWorkStation.Value
                    && __instance.IsJanitor())
                {
                    __instance.NetworkisFired = false;
                    __instance.isFired = false;
                }

                // Spoof Role to Hide Slackers
                if (ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                {
                    if (__instance.NetworkplayerRole == PlayerRole.Slacker)
                        __instance.NetworkplayerRole = PlayerRole.Specialist;
                    if (__instance.NetworkoriginalPlayerRole == PlayerRole.Slacker)
                        __instance.NetworkoriginalPlayerRole = PlayerRole.Specialist;
                }
            }
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
        private static bool InvokeUserCode_CmdSetLocalPlayer__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Validate Sender
            LobbyPlayer sender = __0.TryCast<LobbyPlayer>();
            if ((sender == null)
                || sender.WasCollected
                || LobbyManager.instance.gameStarted)
                return false;

            // Run Game Command
            sender.UserCode_CmdSetLocalPlayer__NetworkConnectionToClient(__2);

            // Prevent Original
            return false;
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdReplacePlayerWithSpectator__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdReplacePlayerWithSpectator__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
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
                || (sender.NetworkplayerController == null)
                || sender.NetworkplayerController.WasCollected)
                return false;

            // Run Game Command
            sender.UserCode_CmdReplacePlayerWithSpectator__NetworkConnectionToClient(__2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdSetSubRole__SubRole))]
        private static bool InvokeUserCode_CmdSetSubRole__SubRole_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get LobbyPlayer
            LobbyPlayer targetPlayer = __0.TryCast<LobbyPlayer>();
            if ((targetPlayer == null)
                || targetPlayer.WasCollected
                || (targetPlayer.NetworkplayerRole == PlayerRole.Manager))
                return false;

            // Validate Manager Role
            LobbyPlayer sender = __2.identity.GetComponent<LobbyPlayer>();
            if ((sender == null)
                || sender.WasCollected
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

            // Get Last Assistant
            LobbyPlayer lastAssistant = LobbyManager.instance.GetAssistantPlayer();

            // Remove Assistant Role from All Others
            if (requestedRole == SubRole.Assistant)
                foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
                {
                    // Get Old Player
                    LobbyPlayer oldPlayer = networkIdentity.GetComponent<LobbyPlayer>();
                    if ((oldPlayer == null)
                        || (oldPlayer == targetPlayer)
                        || (oldPlayer.NetworksubRole != SubRole.Assistant))
                        continue;

                    // Reset Role
                    oldPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);
                }

            // Run Game Command
            targetPlayer.UserCode_CmdSetSubRole__SubRole(requestedRole);

            if ((lastAssistant != null)
                && !lastAssistant.WasCollected)
                GameManager.instance.RpcSetAssistant(lastAssistant.netIdentity, targetPlayer.netIdentity);
            else
                GameManager.instance.RpcSetAssistant(null, targetPlayer.netIdentity);

            // Prevent Original
            return false;
        }
    }
}
