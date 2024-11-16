﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using System.Collections;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_GameManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.NetworktargetGameState), MethodType.Setter)]
        private static void NetworktargetGameState_set_Prefix(GameManager __instance, ref int __0)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return;
            if (!NetworkServer.activeHost)
                return;
            if (__0 != (int)GameStates.GameFinished)
                return;
            if (InteractionSecurity.GetWinner(__instance) != PlayerRole.None)
                return;
            __0 = (int)GameStates.InGame;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.EndGameIfFinished))]
        private static bool EndGameIfFinished_Prefix(GameManager __instance)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return true;
            if (!NetworkServer.activeHost)
                return true;
            if (InteractionSecurity.GetWinner(__instance) == PlayerRole.None)
                return false;

            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedLobbyPlayers)
            {
                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if (!player.isFired
                    && InteractionSecurity.IsSlacker(player))
                {
                    player.NetworkplayerRole = PlayerRole.Slacker;
                    player.NetworkoriginalPlayerRole = PlayerRole.Slacker;
                    player.CustomRpcSetPlayerRole(PlayerRole.Slacker, false);
                }
            }

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ServerFirePlayer))]
        private static bool ServerFirePlayer_Prefix(
            GameManager __instance,
            NetworkIdentity __0) // player
        {
            // Get LobbyPlayer
            LobbyPlayer player = __0.GetComponent<LobbyPlayer>();
            if (player == null)
                return true;
            if (player.NetworkisFired)
                return true;

            // Send Role
            if (ConfigHandler.Gameplay.HideSlackersFromClients.Value
                && __instance.revealRoleAfterFiring
                && InteractionSecurity.IsSlacker(player))
            {
                player.NetworkplayerRole = PlayerRole.Slacker;
                player.NetworkoriginalPlayerRole = PlayerRole.Slacker;
                player.CustomRpcSetPlayerRole(PlayerRole.Slacker, false);
            }

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdAddProductivityFromTaskCompletion__PlayerRole))]
        private static bool InvokeUserCode_CmdAddProductivityFromTaskCompletion__PlayerRole_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Validate
            if ((__2 == null)
                || __2.WasCollected)
                return false;

            // Get GameManager
            GameManager manager = __0.TryCast<GameManager>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            LobbyPlayer player = __2.GetLobbyPlayerFromConnection();

            // Run Game Command
            manager.StartCoroutine(CoroutineAddProductivityFromTaskCompletion(manager, player));

            // Prevent Original
            return false;
        }

        private static IEnumerator CoroutineAddProductivityFromTaskCompletion(GameManager manager, LobbyPlayer player)
        {
            if (manager.delayedScoreOnSlackerTasks)
                yield return new WaitForSeconds(Random.Range(0f, 30f));

            float num = 0f;
            if (InteractionSecurity.IsSlacker(player))
                num = manager.productivityPerSlackerTask / InteractionSecurity.GetAmountOfUnfiredSlackers(LobbyManager.instance);
            else if (player.NetworkplayerRole == PlayerRole.Specialist)
                num = manager.productivityPerSpecialistTask / InteractionSecurity.GetAmountOfUnfiredSpecialists(LobbyManager.instance);
            else if (player.NetworkplayerRole == PlayerRole.Manager)
                num = manager.productivityPerManagerTask;

            manager.productivity += num;

            yield break;
        }
    }
}
