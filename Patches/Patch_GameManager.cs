using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_GameManager
    {

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

            if (!__1)
                __instance.EndGameIfFinished();

            // Prevent Original
            return false;
        }
    }
}
