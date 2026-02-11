using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Fixes.TerminationRework.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UnRegisterPlayer))]
        private static void UnRegisterPlayer_Prefix(LobbyManager __instance,
            NetworkIdentity __0)
        {
            // Validate Server
            if (!__instance.isServer
                || !NetworkServer.activeHost)
                return;

            // Get Lobby Player
            LobbyPlayer lobbyPlayer = __0.GetComponent<LobbyPlayer>();
            if (lobbyPlayer == null
                || lobbyPlayer.WasCollected)
                return;

            // Validate Game State
            if (__instance.gameStarted
                && !lobbyPlayer.IsGhost())
            {
                bool isJanitor = lobbyPlayer.IsJanitor();
                PlayerRole playerRole = lobbyPlayer.playerRole;

                if (!isJanitor
                    || Extras.MoreRoleSettings.ModuleConfig.Instance.AllowJanitorsToKeepWorkStation.Value)
                    lobbyPlayer.ServerSetWorkStation(null, playerRole, true);

                // Check Setting
                if ((ModuleConfig.Instance.PlayerLeavesReduceTerminations.Value != ModuleConfig.eTermType.Never)
                    && !isJanitor
                    && playerRole != PlayerRole.Manager
                    && GameManager.instance != null
                    && !GameManager.instance.WasCollected)
                {
                    if (ModuleConfig.Instance.PlayerLeavesReduceTerminations.Value == ModuleConfig.eTermType.Every_Other_Disconnect)
                    {
                        if (ModuleMain._canTerminate)
                        {
                            ModuleMain._canTerminate = false;
                            return;
                        }
                        ModuleMain._canTerminate = true;
                    }
                    else
                        ModuleMain.ResetTerminationChance();

                    // Get Original Count
                    int slackerCount = GameManager.instance.NetworkstartSlackers;
                    int specialistCount = GameManager.instance.NetworkstartSpecialists;

                    // Get Player Role
                    if (lobbyPlayer.IsSlacker())
                        slackerCount--;
                    else if (playerRole == PlayerRole.Specialist)
                        specialistCount--;

                    // Clamp Count
                    if (slackerCount < 0)
                        slackerCount = 0;
                    if (specialistCount < 0)
                        specialistCount = 0;

                    // Apply New Counts
                    GameManager.instance.NetworkstartSlackers = slackerCount;
                    GameManager.instance.NetworkstartSpecialists = specialistCount;
                }
            }
        }
    }
}
