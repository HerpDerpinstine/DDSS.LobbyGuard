using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PhoneManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdAnswerCall__String__String))]
        private static bool InvokeUserCode_CmdAnswerCall__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Phone
            PhoneManager phone = __0.TryCast<PhoneManager>();

            NetworkIdentity sender = __2.identity;

            // Get Player
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();

            // Run Security
            PhoneSecurity.OnCallAnswer(phone, caller, receiver);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdCancelCall__String__String))]
        private static bool InvokeUserCode_CmdCancelCall__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Phone
            PhoneManager phone = __0.TryCast<PhoneManager>();

            NetworkIdentity sender = __2.identity;

            // Get Player
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Caller Number
            string caller = phone.GetPhoneNumber(lobbyPlayer);

            // Get New Receiver
            __1.SafeReadString();
            string receiver = __1.SafeReadString();

            // Run Security
            PhoneSecurity.OnCallCancel(phone, caller, receiver);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdEndCall__String__String))]
        private static bool InvokeUserCode_CmdEndCall__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Phone
            PhoneManager phone = __0.TryCast<PhoneManager>();

            NetworkIdentity sender = __2.identity;

            // Get Player
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();

            // Run Security
            PhoneSecurity.OnCallEnd(phone, caller, receiver);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdDeclineCall__String__String))]
        private static bool InvokeUserCode_CmdDeclineCall__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Phone
            PhoneManager phone = __0.TryCast<PhoneManager>();

            NetworkIdentity sender = __2.identity;

            // Get Player
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();

            // Run Security
            PhoneSecurity.OnCallDecline(phone, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
