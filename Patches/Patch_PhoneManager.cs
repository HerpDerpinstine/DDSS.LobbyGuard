using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
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
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chairIdentity = controller.NetworkcurrentChair;
            if ((chairIdentity == null)
                || chairIdentity.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = chairIdentity.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.NetworkownerLobbyPlayer == null)
                || workStation.NetworkownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                caller = receiver;

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
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chairIdentity = controller.NetworkcurrentChair;
            if ((chairIdentity == null)
                || chairIdentity.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = chairIdentity.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.NetworkownerLobbyPlayer == null)
                || workStation.NetworkownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Caller Number
            string caller = phone.GetPhoneNumber(lobbyPlayer);
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get New Receiver
            __1.SafeReadString();
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                receiver = caller;

            // Run Security
            PhoneSecurity.OnCallCancel(phone, receiver, caller);
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
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chairIdentity = controller.NetworkcurrentChair;
            if ((chairIdentity == null)
                || chairIdentity.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = chairIdentity.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.NetworkownerLobbyPlayer == null)
                || workStation.NetworkownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                caller = receiver;

            // Run Security
            PhoneSecurity.OnCallEnd(phone, caller, receiver);
            PhoneSecurity.OnCallEnd(phone, receiver, caller);

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
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chairIdentity = controller.NetworkcurrentChair;
            if ((chairIdentity == null)
                || chairIdentity.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = chairIdentity.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.NetworkownerLobbyPlayer == null)
                || workStation.NetworkownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string receiver = phone.GetPhoneNumber(lobbyPlayer);
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Get New Caller
            __1.SafeReadString();
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                caller = receiver;

            // Run Security
            PhoneSecurity.OnCallDecline(phone, caller, receiver);
            PhoneSecurity.OnCallDecline(phone, receiver, caller);

            // Prevent Original
            return false;
        }
    }
}
