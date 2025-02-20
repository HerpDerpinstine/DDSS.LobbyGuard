using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.MoreJanitorSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool UserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(NetworkIdentity __0, bool __1)
        {
            // Get Player
            LobbyPlayer player = __0.GetLobbyPlayer();
            if (player == null
                || player.WasCollected)
                return false;

            // Validate Role
            var role = player.playerRole;
            if (role == PlayerRole.Janitor)
            {
                if (__1) // Locked
                {
                    if (!ModuleConfig.Instance.AllowJanitorsToLockDoors.Value)
                        return false;
                }
                else // Unlocked
                {
                    if (!ModuleConfig.Instance.AllowJanitorsToUnlockDoors.Value)
                        return false;
                }
            }

            // Run Original
            return true;
        }
    }
}
