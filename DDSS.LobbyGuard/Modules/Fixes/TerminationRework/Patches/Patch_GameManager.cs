using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;

namespace DDSS_LobbyGuard.Modules.Fixes.TerminationRework.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_GameManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.ReplaceManager))]
        private static bool ReplaceManager_Prefix(GameManager __instance)
        {
            // Find New Manager
            LobbyPlayer newManager = __instance.FindNewManager();
            if (newManager == null
                || newManager.WasCollected)
                return false;

            // Get Game Rule
            bool flag = GameRulesSettingsManager.instance.GetSetting("Fire the old manager after vote") == 1f;

            // Get Old Manager
            LobbyPlayer oldManager = LobbyManager.instance.GetManagerPlayer();
            if (oldManager != null
                && !oldManager.WasCollected)
            {
                bool janitorsKeepWorkstation = MoreJanitorSettingsConfig.Instance.AllowJanitorsToKeepWorkStation.Value;
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
            if (oldManager != null
                && !oldManager.WasCollected)
                __instance.RpcDisplayNewRoles(newManager.netIdentity, oldManager.netIdentity);
            else
                __instance.RpcDisplayNewRoles(newManager.netIdentity, null);

            if (newManager.subRole == SubRole.HrRep
                && __instance.NetworkuseHrRep)
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
            if ((__0 != null)
                && !__0.WasCollected
                && (__instance.protectedLobbyPlayer != null)
                && !__instance.protectedLobbyPlayer.WasCollected
                && (__instance.protectedLobbyPlayer == __0))
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __0.GetComponent<LobbyPlayer>();
            if (player == null
                || player.WasCollected
                || player.IsGhost())
                return false;

            // Get Janitor Count
            bool wasHR = player.subRole == SubRole.HrRep;
            var janitorList = LobbyManager.instance.GetJanitorPlayers();
            bool flag = !player.IsJanitor()
                && __instance.NetworkjanitorAmount > 0
                && janitorList.Count < __instance.NetworkjanitorAmount;

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            // End Meeting
            if (!__1)
                __instance.ServerFinnishMeeting();

            // Reset Workstation
            bool janitorsKeepWorkstation = MoreJanitorSettingsConfig.Instance.AllowJanitorsToKeepWorkStation.Value;
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
            player.isFired = !janitorsKeepWorkstation || !flag;

            if (!flag)
                player.ServerReplacePlayerWithSpectator(__0.connectionToClient);
            if (__instance.NetworkuseHrRep
                && wasHR)
                __instance.SelectNewHrRep();

            // Reset Vote
            VoteBoxController.instance.ServerResetVote();

            // End Match if Winner is Found
            if (!__1)
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
        }

    }
}