using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
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
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ReplaceManager))]
        private static bool ReplaceManager_Prefix(GameManager __instance)
        {
            // Find New Manager
            LobbyPlayer lobbyPlayer = __instance.FindNewManager();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return false;

            // Get Old Manager
            LobbyPlayer managerPlayer = LobbyManager.instance.GetManagerPlayer();

            // Reset Old Manager
            if ((managerPlayer != null)
                && !managerPlayer.WasCollected)
            {
                if (ConfigHandler.Gameplay.HideSlackersFromClients.Value
                    && InteractionSecurity.IsSlacker(lobbyPlayer))
                    managerPlayer.ServerSetPlayerRole(PlayerRole.Slacker);
                else
                    managerPlayer.ServerSetPlayerRole(lobbyPlayer.playerRole);
            }

            // Set New Manager
            lobbyPlayer.ServerSetPlayerRole(PlayerRole.Manager);

            // Apply Game Rule
            bool flag = GameRulesSettingsManager.instance.GetSetting("Fire the old manager after vote") == 1f;
            if ((managerPlayer != null)
                && !managerPlayer.WasCollected)
            {
                if (flag)
                {
                    // Fire Old Manager
                    __instance.ServerFirePlayer(managerPlayer.netIdentity, true, false);
                    managerPlayer.NetworkisFired = true;
                }
                else
                {
                    // Assign Old Manager Workstation
                    managerPlayer.ServerSetWorkStation(lobbyPlayer.NetworkworkStationController, managerPlayer.playerRole, false);

                    // Reset Old Manager Tasks
                    TaskController managerComponent = managerPlayer.GetComponent<TaskController>();
                    if (managerComponent != null)
                        managerComponent.RpcClearTaskQueue();
                }
            }

            // Assign New Manager Workstation
            lobbyPlayer.ServerSetWorkStation(__instance.managerWorkStationController, PlayerRole.Manager, flag);

            // Reset New Manager Tasks
            TaskController component = lobbyPlayer.GetComponent<TaskController>();
            if (component != null)
                component.RpcClearTaskQueue();

            // Display New Roles
            if ((managerPlayer != null)
                && !managerPlayer.WasCollected)
                __instance.RpcDisplayNewRoles(lobbyPlayer.netIdentity, managerPlayer.netIdentity);
            else
                __instance.RpcDisplayNewRoles(lobbyPlayer.netIdentity, null);

            // Prevent Original
            return false;
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
                return true;
            if (player.NetworkisFired
                && (player.NetworkplayerRole != PlayerRole.Janitor))
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

            // Reset Vote
            if (__instance.isServer)
                VoteBoxController.instance.ServerResetVote();

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            // End Meeting
            if (!__1)
                __instance.ServerFinnishMeeting();

            // Get Janitor Count
            var janitorList = LobbyManager.instance.GetJanitorPlayers();
            bool flag = !player.NetworkisFired
                && (player.NetworkplayerRole != PlayerRole.Janitor)
                && (__instance.NetworkjanitorAmount > 0)
                && (janitorList.Count < __instance.NetworkjanitorAmount);

            // Reset Workstation
            if (__2)
                player.ServerSetWorkStation(null, player.NetworkplayerRole, true);

            // Fire Player
            player.RpcFirePlayer(true, !flag, player.NetworkplayerRole);

            // Assign Janitor Role
            if (flag)
                player.ServerSetPlayerRole(PlayerRole.Janitor);

            // Apply Fired State
            player.NetworkisFired = true;

            // End Match if Winner is Found
            if (!__1
                && (InteractionSecurity.GetWinner(__instance) != PlayerRole.None))
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdAddProductivityFromTaskCompletion__PlayerRole))]
        private static bool InvokeUserCode_CmdAddProductivityFromTaskCompletion__PlayerRole_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Validate
            if ((__2 == null)
                || __2.WasCollected)
                return false;

            // Check Setting
            if (!ConfigHandler.Gameplay.ProductivityFromTaskCompletion.Value)
                return false;

            // Get GameManager
            GameManager manager = __0.TryCast<GameManager>();
            if ((manager == null)
                || manager.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            LobbyPlayer player = __2.GetLobbyPlayerFromConnection();
            if ((player == null)
                || player.WasCollected)
                return false;

            // Run Game Command
            manager.StartCoroutine(CoroutineAddProductivityFromTaskCompletion(manager, player));

            // Prevent Original
            return false;
        }

        private static IEnumerator CoroutineAddProductivityFromTaskCompletion(GameManager manager, LobbyPlayer player)
        {
            if (manager.NetworkdelayedScoreOnSlackerTasks)
                yield return new WaitForSeconds(Random.Range(0f, 30f));

            if ((manager != null)
                && !manager.WasCollected
                && (player != null)
                && !player.WasCollected
                && (LobbyManager.instance != null)
                && !LobbyManager.instance.WasCollected)
            {
                float num = 0f;
                if (InteractionSecurity.IsSlacker(player))
                    num = manager.NetworkproductivityPerSlackerTask / InteractionSecurity.GetAmountOfUnfiredSlackers(LobbyManager.instance);
                else if (player.NetworkplayerRole == PlayerRole.Specialist)
                    num = manager.NetworkproductivityPerSpecialistTask / InteractionSecurity.GetAmountOfUnfiredSpecialists(LobbyManager.instance);
                else if (player.NetworkplayerRole == PlayerRole.Manager)
                    num = manager.NetworkproductivityPerManagerTask;

                manager.productivity += num;
            }

            yield break;
        }
    }
}
