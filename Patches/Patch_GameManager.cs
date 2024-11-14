using DDSS_LobbyGuard.Config;
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

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ServerFirePlayer))]
        private static bool ServerFirePlayer_Prefix(
            GameManager __instance,
            NetworkIdentity __0, // player
            bool __1, // onlyDemoteOrFire
            bool __2) // resetDesk
        {
            // Get LobbyPlayer
            LobbyPlayer player = __0.GetComponent<LobbyPlayer>();
            if (player == null)
                return false;
            if (player.NetworkisFired
                && (player.NetworkplayerRole != PlayerRole.Janitor))
                return false;

            // Send Role
            if (ConfigHandler.Gameplay.HideSlackersFromClients.Value
                && __instance.revealRoleAfterFiring
                && InteractionSecurity.IsSlacker(player))
            {
                player.NetworkplayerRole = PlayerRole.Slacker;
                player.NetworkoriginalPlayerRole = PlayerRole.Slacker;
                player.CustomRpcSetPlayerRole(PlayerRole.Slacker, false);
            }

            if (__instance.isServer)
                VoteBoxController.instance.ServerResetVote();

            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            if (!__1)
                __instance.ServerFinnishMeeting();

            var janitorList = LobbyManager.instance.GetJanitorPlayers();
            bool flag = !player.NetworkisFired
                && (player.NetworkplayerRole != PlayerRole.Janitor)
                && (__instance.NetworkjanitorAmount > 0)
                && (janitorList.Count < __instance.NetworkjanitorAmount);

            if (__2)
                player.ServerSetWorkStation(null, player.NetworkplayerRole, true);

            player.RpcFirePlayer(true, !flag, player.NetworkplayerRole);

            if (flag)
                player.ServerSetPlayerRole(PlayerRole.Janitor);

            player.NetworkisFired = true;

            if (!__1 && (InteractionSecurity.GetWinner(__instance) != PlayerRole.None))
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
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
            LobbyPlayer player = sender.GetComponent<LobbyPlayer>();

            // Run Game Command
            if (player != null)
                manager.StartCoroutine(CoroutineAddProductivityFromTaskCompletion(manager, player));

            // Prevent Original
            return false;
        }

        private static IEnumerator CoroutineAddProductivityFromTaskCompletion(GameManager manager, LobbyPlayer player)
        {
            if (manager.delayedScoreOnSlackerTasks)
                yield return new WaitForSeconds(Random.Range(0f, 30f));
            if (player == null)
                yield break;

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
