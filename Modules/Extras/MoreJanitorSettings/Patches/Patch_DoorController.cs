using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Extras.MoreJanitorSettings.Patches
{
    [HarmonyPatch]
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool UserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(NetworkIdentity __0, bool __1)
        {
            LobbyPlayer player = __0.GetLobbyPlayer();
            if ((player == null)
                || player.WasCollected)
                return false;

            var role = player.playerRole;
            if (role != PlayerRole.Janitor)
                return true;

            if (__1)
            {
                if (!ModuleConfig.Instance.AllowJanitorsToLockDoors.Value)
                    return false;
            }
            else
            {
                if (!ModuleConfig.Instance.AllowJanitorsToUnlockDoors.Value)
                    return false;
            }

            // Run Original
            return true;
        }
    }
}
