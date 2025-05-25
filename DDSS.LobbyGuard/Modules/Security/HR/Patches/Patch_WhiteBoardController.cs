using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Security.HR.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_WhiteBoardController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WhiteBoardController), nameof(WhiteBoardController.InvokeUserCode_CmdProtectPlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdProtectPlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get WhiteBoardController
            WhiteBoardController whiteBoard = __0.TryCast<WhiteBoardController>();
            if ((whiteBoard == null)
                || whiteBoard.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.HrRep)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, whiteBoard.transform.position))
                return false;

            __1.SafeReadNetworkIdentity();
            NetworkIdentity target = __1.SafeReadNetworkIdentity();
            if ((target == null)
                || target.WasCollected)
                return false;

            GameManager.instance.ServerSetProtectedPlayer(target);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WhiteBoardController), nameof(WhiteBoardController.InvokeUserCode_CmdStopProtectPlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStopProtectPlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get WhiteBoardController
            WhiteBoardController whiteBoard = __0.TryCast<WhiteBoardController>();
            if ((whiteBoard == null)
                || whiteBoard.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.HrRep)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, whiteBoard.transform.position))
                return false;

            __1.SafeReadNetworkIdentity();
            NetworkIdentity target = __1.SafeReadNetworkIdentity();
            if ((target == null)
                || target.WasCollected)
                return false;

            GameManager.instance.ServerResetProtectedPlayer();

            // Prevent Original
            return false;
        }
    }
}
