using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_WhiteBoardController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WhiteBoardController), nameof(WhiteBoardController.InvokeUserCode_CmdFirePlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFirePlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected
                || (GameManager.instance.NetworktargetGameState != (int)GameStates.Meeting))
                return false;

            // Get WhiteBoardController
            WhiteBoardController whiteBoard = __0.TryCast<WhiteBoardController>();
            if ((whiteBoard == null)
                || whiteBoard.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, whiteBoard.transform.position))
                return false;

            NetworkIdentity target = __1.SafeReadNetworkIdentity();
            if ((target == null)
                || target.WasCollected)
                return false;

            whiteBoard.UserCode_CmdFirePlayer__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient(target, sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WhiteBoardController), nameof(WhiteBoardController.InvokeUserCode_CmdCallMeeting__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCallMeeting__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected
                || ((GameManager.instance.NetworktargetGameState != (int)GameStates.InGame)
                    && (GameManager.instance.NetworktargetGameState != (int)GameStates.Tutorial)))
                return false;

            // Get WhiteBoardController
            WhiteBoardController whiteBoard = __0.TryCast<WhiteBoardController>();
            if ((whiteBoard == null)
                || whiteBoard.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, whiteBoard.transform.position))
                return false;

            whiteBoard.UserCode_CmdCallMeeting__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WhiteBoardController), nameof(WhiteBoardController.InvokeUserCode_CmdEndMeeting__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEndMeeting__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected
                || (GameManager.instance.NetworktargetGameState != (int)GameStates.Meeting))
                return false;

            // Get WhiteBoardController
            WhiteBoardController whiteBoard = __0.TryCast<WhiteBoardController>();
            if ((whiteBoard == null)
                || whiteBoard.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, whiteBoard.transform.position))
                return false;

            whiteBoard.UserCode_CmdEndMeeting__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

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
