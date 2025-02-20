using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.MoreJanitorSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CCTVController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CCTVController), nameof(CCTVController.UserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient))]
        private static bool UserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient_Prefix(
            CCTVController __instance,
            NetworkIdentity __0,
            NetworkConnectionToClient __1)
        {
            LobbyPlayer lobbyPlayer = __0.GetLobbyPlayer();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            if (!ModuleConfig.Instance.AllowJanitorsToUpdateCCTV.Value
                && lobbyPlayer.IsJanitor())
                return false;

            // Run Original
            return true;
        }
    }
}