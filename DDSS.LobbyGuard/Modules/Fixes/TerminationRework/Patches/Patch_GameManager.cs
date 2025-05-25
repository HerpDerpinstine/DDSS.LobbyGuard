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
                bool janitorsKeepWorkstation = Extras.MoreJanitorSettings.ModuleConfig.Instance.AllowJanitorsToKeepWorkStation.Value;
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

            if (newManager.subRole == SubRole.HrRep)
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

            var current_role = player.playerRole;
            var current_subrole = player.subRole;

            bool is_hr_rep = current_subrole == SubRole.HrRep;

            bool is_janitor = player.IsJanitor();
            int max_janitor_count = __instance.NetworkjanitorAmount;
            var janitorList = LobbyManager.instance.GetJanitorPlayers();

            bool should_janitors_keep_workstations = Extras.MoreJanitorSettings.ModuleConfig.Instance.AllowJanitorsToKeepWorkStation.Value;
            bool should_become_janitor = !is_janitor
                && (max_janitor_count > 0)
                && (janitorList.Count < __instance.NetworkjanitorAmount);

            // Reset Termination Timer
            __instance.RpcResetTerminationTimer(__instance.terminationMaxTime);

            // Reset Vote
            VoteBoxController.instance.ServerResetVote();

            // End Meeting
            if (!__1)
                __instance.ServerFinnishMeeting();

            // Fire Player
            player.RpcFirePlayer(true, should_become_janitor 
                    ? current_role 
                    : player.originalPlayerRole, 
                current_role, 
                !should_become_janitor);

            // Apply Fired State
            player.isFired = !should_janitors_keep_workstations || !should_become_janitor;
            player.originalPlayerRole = current_role;

            // Check if should become Janitor or Ghost
            if (should_become_janitor)
            {
                // Reset Workstation
                if (__2 && !should_janitors_keep_workstations)
                    player.ServerSetWorkStation(null, current_role, true);

                // Apply New Role
                player.ServerSetPlayerRole(PlayerRole.Janitor);
            }
            else
            {
                // Reset Workstation
                player.ServerSetWorkStation(null, current_role, true);

                // Apply New Role
                player.ServerSetPlayerRole(PlayerRole.None);

                // Replace Player with Ghost
                player.ServerReplacePlayerWithSpectator(__0.connectionToClient);
            }

            // Select New HR Rep
            if (is_hr_rep)
                __instance.SelectNewHrRep();

            // End Match if Winner is Found
            if (!__1)
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
        }

    }
}