using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using System.Collections;
using System.Collections.Generic;
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

            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
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
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.FindNewManager))]
        private static bool FindNewManager_Prefix(GameManager __instance, ref LobbyPlayer __result)
        {
            // Get All Players
            List<NetworkIdentity> list = LobbyManager.instance.GetAllPlayers();

            // Get List of Specialists
            List<LobbyPlayer> validPlayers = new();
            foreach (NetworkIdentity networkIdentity in list)
            {
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected)
                    continue;

                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected
                    || player.IsGhost()
                    || player.IsJanitor()
                    || (player.NetworkplayerRole != PlayerRole.Specialist)
                    || InteractionSecurity.IsSlacker(player))
                    continue;

                validPlayers.Add(player);
            }

            // Validate Player Count
            int playerCount = validPlayers.Count;
            if (playerCount <= 0)
                return false;

            // Shuffle List
            validPlayers.Shuffle();
            __result = validPlayers[0];

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ReplaceManager))]
        private static bool ReplaceManager_Prefix(GameManager __instance)
        {
            // Find New Manager
            LobbyPlayer newManager = __instance.FindNewManager();
            if ((newManager == null)
                || newManager.WasCollected)
                return false;

            // Get Game Rule
            bool flag = GameRulesSettingsManager.instance.GetSetting("Fire the old manager after vote") == 1f;

            // Get Old Manager
            LobbyPlayer oldManager = LobbyManager.instance.GetManagerPlayer();
            if ((oldManager != null)
                && !oldManager.WasCollected)
            {
                if (flag)
                {
                    // Fire Old Manager
                    __instance.ServerFirePlayer(oldManager.netIdentity, true, false);
                    oldManager.NetworkisFired = true;
                }
                else
                {
                    if (ConfigHandler.Gameplay.HideSlackersFromClients.Value
                        && InteractionSecurity.IsSlacker(oldManager))
                        oldManager.ServerSetPlayerRole(PlayerRole.Slacker);
                    else
                        oldManager.ServerSetPlayerRole(newManager.playerRole);
                }

                // Assign Old Manager Workstation
                oldManager.ServerSetWorkStation(newManager.NetworkworkStationController, oldManager.playerRole, false);

                // Reset Old Manager Tasks
                TaskController managerComponent = oldManager.GetComponent<TaskController>();
                if (managerComponent != null)
                    managerComponent.RpcClearTaskQueue();
            }

            // Set New Manager
            newManager.ServerSetPlayerRole(PlayerRole.Manager);
            newManager.ServerSetWorkStation(__instance.managerWorkStationController, PlayerRole.Manager, flag);

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);
            
            // Reset Vote
            VoteBoxController.instance.ServerResetVote();

            // Reset New Manager Tasks
            TaskController component = newManager.GetComponent<TaskController>();
            if (component != null)
                component.RpcClearTaskQueue();

            // Display New Roles
            if ((oldManager != null)
                && !oldManager.WasCollected)
                __instance.RpcDisplayNewRoles(newManager.netIdentity, oldManager.netIdentity);
            else
                __instance.RpcDisplayNewRoles(newManager.netIdentity, null);

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
            if ((player == null)
                || player.WasCollected
                || player.IsGhost())
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

            // Get Janitor Count
            var janitorList = LobbyManager.instance.GetJanitorPlayers();
            bool flag = !player.IsJanitor()
                && (__instance.NetworkjanitorAmount > 0)
                && (janitorList.Count < __instance.NetworkjanitorAmount);

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            // End Meeting
            if (!__1)
                __instance.ServerFinnishMeeting();

            // Reset Workstation
            if (__2)
                player.ServerSetWorkStation(null, player.NetworkplayerRole, true);

            // Fire Player
            player.RpcFirePlayer(true, !flag, player.NetworkplayerRole);

            // Assign Janitor Role
            if (flag)
                player.ServerSetPlayerRole(PlayerRole.Janitor);
            else
                player.ServerSetPlayerRole(PlayerRole.None);

            // Apply Fired State
            player.NetworkisFired = true;

            // Reset Vote
            if (__instance.isServer)
                VoteBoxController.instance.ServerResetVote();

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
            if ((sender == null)
                || sender.WasCollected)
                return false;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost()
                || lobbyPlayer.IsJanitor())
                return false;

            // Run Game Command
            manager.StartCoroutine(CoroutineAddProductivityFromTaskCompletion(manager, InteractionSecurity.IsSlacker(lobbyPlayer) ? PlayerRole.Slacker : lobbyPlayer.NetworkplayerRole));

            // Prevent Original
            return false;
        }

        private static IEnumerator CoroutineAddProductivityFromTaskCompletion(GameManager manager, PlayerRole role)
        {
            if (manager.NetworkdelayedScoreOnSlackerTasks)
                yield return new WaitForSeconds(Random.Range(0f, 30f));

            if ((manager != null)
                && !manager.WasCollected
                && (LobbyManager.instance != null)
                && !LobbyManager.instance.WasCollected)
            {
                float num = 0f;
                if (role == PlayerRole.Slacker)
                    num = manager.NetworkproductivityPerSlackerTask / InteractionSecurity.GetAmountOfUnfiredSlackers(LobbyManager.instance);
                else if (role == PlayerRole.Specialist)
                    num = manager.NetworkproductivityPerSpecialistTask / InteractionSecurity.GetAmountOfUnfiredSpecialists(LobbyManager.instance);
                else if (role == PlayerRole.Manager)
                    num = manager.NetworkproductivityPerManagerTask;

                manager.productivity += num;
            }

            yield break;
        }
    }
}
