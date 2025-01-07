using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.WorkStation.Phone;

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
            NetworkIdentity sender = __2.identity;

            // Get Phone
            PhoneManager phoneManager = __0.TryCast<PhoneManager>();
            if ((phoneManager == null)
                || phoneManager.WasCollected)
                return false;

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

            NetworkIdentity chair = controller.NetworkcurrentChair;
            if ((chair == null)
                || chair.WasCollected)
                return false;

            WorkStationController station = chair.GetComponent<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            PhoneController phone = station.phoneController;
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Enforce Receiver Number
            string receiver = phone.phoneNumber;
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            string caller = phone.NetworkreceivingCall;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Run Security
            PhoneSecurity.OnCallAnswer(phoneManager, caller, receiver);

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
            NetworkIdentity sender = __2.identity;
            
            // Get Phone
            PhoneManager phoneManager = __0.TryCast<PhoneManager>();
            if ((phoneManager == null)
                || phoneManager.WasCollected)
                return false;

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

            NetworkIdentity chair = controller.NetworkcurrentChair;
            if ((chair == null)
                || chair.WasCollected)
                return false;

            WorkStationController station = chair.GetComponent<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            PhoneController phone = station.phoneController;
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Enforce Caller Number
            string caller = phone.phoneNumber;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get New Receiver
            string receiver = phone.NetworkcallingNumber;
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Run Security
            PhoneSecurity.OnCallCancel(phoneManager, caller, receiver);

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
            NetworkIdentity sender = __2.identity;

            // Get Phone
            PhoneManager phoneManager = __0.TryCast<PhoneManager>();
            if ((phoneManager == null)
                || phoneManager.WasCollected)
                return false;

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

            NetworkIdentity chair = controller.NetworkcurrentChair;
            if ((chair == null)
                || chair.WasCollected)
                return false;

            WorkStationController station = chair.GetComponent<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            PhoneController phone = station.phoneController;
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Enforce Receiver Number
            string receiver = phone.NetworkcallingNumber;
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            string caller = phone.phoneNumber;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Run Security
            PhoneSecurity.OnCallEnd(phoneManager, caller, receiver);

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
            NetworkIdentity sender = __2.identity;

            // Get Phone
            PhoneManager phoneManager = __0.TryCast<PhoneManager>();
            if ((phoneManager == null)
                || phoneManager.WasCollected)
                return false;

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

            NetworkIdentity chair = controller.NetworkcurrentChair;
            if ((chair == null)
                || chair.WasCollected)
                return false;

            WorkStationController station = chair.GetComponent<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            PhoneController phone = station.phoneController;
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Enforce Receiver Number
            string receiver = phone.phoneNumber;
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            string caller = phone.NetworkreceivingCall;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Run Security
            PhoneSecurity.OnCallDecline(phoneManager, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
