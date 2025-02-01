using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_GameManager
    {
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
                    || (player.playerRole != PlayerRole.Specialist)
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
                bool janitorsKeepWorkstation = ConfigHandler.Gameplay.AllowJanitorsToKeepWorkStation.Value;
                if (flag)
                {
                    // Fire Old Manager
                    __instance.ServerFirePlayer(oldManager.netIdentity, true, !janitorsKeepWorkstation);
                    oldManager.isFired = true;
                }
                else
                {
                    oldManager.ServerSetPlayerRole(newManager.playerRole);
                }

                // Assign Old Manager Workstation
                oldManager.ServerSetWorkStation(newManager.NetworkworkStationController, oldManager.playerRole);

                // Reset Old Manager Tasks
                TaskController managerComponent = oldManager.GetComponent<TaskController>();
                if (managerComponent != null)
                    managerComponent.ServerClearTaskQueue();
            }

            // Set New Manager
            newManager.ServerSetPlayerRole(PlayerRole.Manager);
            newManager.ServerSetWorkStation(__instance.managerWorkStationController, PlayerRole.Manager);

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            // Reset Vote
            VoteBoxController.instance.ServerResetVote();

            // Reset New Manager Tasks
            TaskController component = newManager.GetComponent<TaskController>();
            if (component != null)
                component.ServerClearTaskQueue();

            // Display New Roles
            if ((oldManager != null)
                && !oldManager.WasCollected)
                __instance.RpcDisplayNewRoles(newManager.netIdentity, oldManager.netIdentity);
            else
                __instance.RpcDisplayNewRoles(newManager.netIdentity, null);

            if ((newManager.subRole == SubRole.HrRep)
                && __instance.NetworkuseHrRep
                && __instance.NetworkselectNewHrRepWhenFired)
                __instance.SelectNewHrRep();
            newManager.ServerSetSubRole(SubRole.None, true);

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
            bool janitorsKeepWorkstation = ConfigHandler.Gameplay.AllowJanitorsToKeepWorkStation.Value;
            if (__2
                && (!flag || !janitorsKeepWorkstation))
                player.ServerSetWorkStation(null, player.playerRole, true);

            // Fire Player

            player.RpcFirePlayer(true, flag ? player.playerRole : player.originalPlayerRole, player.playerRole, !flag);

            // Assign Janitor Role
            player.originalPlayerRole = player.playerRole;
            if (flag)
                player.ServerSetPlayerRole(PlayerRole.Janitor);
            else
                player.ServerSetPlayerRole(PlayerRole.None);

            // Apply Fired State
            player.isFired = (!janitorsKeepWorkstation || !flag);

            if (!flag)
                player.ServerReplacePlayerWithSpectator(__0.connectionToClient);

            bool wasHR = (player.subRole == SubRole.HrRep);
            if (__instance.NetworkuseHrRep
                && __instance.NetworkselectNewHrRepWhenFired
                && wasHR)
                __instance.SelectNewHrRep();

            // Reset Vote
            VoteBoxController.instance.ServerResetVote();

            // End Match if Winner is Found
            if (!__1
                && (__instance.GetWinner() != PlayerRole.None))
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.SelectNewHrRep))]
        private static bool SelectNewHrRep_Prefix(GameManager __instance)
        {
            // Get All Players
            List<NetworkIdentity> list = LobbyManager.instance.GetAllPlayers();

            // Get List of Specialists
            List<NetworkIdentity> validPlayers = new();
            foreach (NetworkIdentity networkIdentity in list)
            {
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected)
                    continue;

                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected)
                    continue;

                // Reset any Current HR
                if (player.subRole == SubRole.HrRep)
                {
                    player.ServerSetSubRole(SubRole.None, true);
                    continue;
                }

                // Skip Ghosts, Janitors, and Manager
                if (player.IsGhost()
                    || player.IsJanitor()
                    || (player.playerRole == PlayerRole.Manager)
                    || (player.subRole != SubRole.None))
                    continue;

                // Add Player to List
                validPlayers.Add(networkIdentity);
            }

            // Validate Player Count
            int playerCount = validPlayers.Count;
            if (playerCount <= 0)
                return false;

            // Shuffle List
            validPlayers.Shuffle();
            __instance.ServerSetHrRep(validPlayers[0]);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdAddProductivityFromTaskCompletion__PlayerRole__NetworkIdentity__NetworkConnectionToClient))]
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
                || lobbyPlayer.IsGhost())
                return false;

            // Run Game Command
            manager.UserCode_CmdAddProductivityFromTaskCompletion__PlayerRole__NetworkIdentity__NetworkConnectionToClient(
                InteractionSecurity.IsSlacker(lobbyPlayer) ? PlayerRole.Slacker : lobbyPlayer.playerRole,
                sender,
                __2);

            // Prevent Original
            return false;
        }
    }
}
