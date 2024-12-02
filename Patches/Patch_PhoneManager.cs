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
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.ServerCall))]
        private static bool ServerCall_Prefix(PhoneManager __instance, string __0, string __1)
        {
            // Validate Input
            if (string.IsNullOrEmpty(__0)
                || string.IsNullOrWhiteSpace(__0)
                || string.IsNullOrEmpty(__1)
                || string.IsNullOrWhiteSpace(__1))
                return false;

            // Prevent Calling Self
            if (__0 == __1)
                return false;

            // Run Security
            PhoneSecurity.OnCallAttempt(__instance, __0, __1);

            // Run Original
            return true;
        }

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

            // Get and Ignore User Input Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected)
                    return false;

                // Validate Sender is on Workstation
                PlayerController controller = __2.identity.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected
                    || (controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                // Validate Chair
                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
                if (string.IsNullOrEmpty(receiver)
                    || string.IsNullOrWhiteSpace(receiver))
                    return false;
            }

            // Get New Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

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

            // Get and Ignore User Input Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Validate Sender is on Workstation
                PlayerController controller = __2.identity.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected
                    || (controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                // Validate Chair
                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                // Enforce Caller Number
                caller = phone.GetPhoneNumber(player);
                if (string.IsNullOrEmpty(caller)
                    || string.IsNullOrWhiteSpace(caller))
                    return false;
            }

            // Get New Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

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

            // Get and Ignore User Input Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Validate Sender is on Workstation
                PlayerController controller = __2.identity.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected
                    || (controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                // Validate Chair
                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
                if (string.IsNullOrEmpty(receiver)
                    || string.IsNullOrWhiteSpace(receiver))
                    return false;
            }

            // Get New Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

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

            // Get and Ignore User Input Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Validate Sender is on Workstation
                PlayerController controller = __2.identity.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected
                    || (controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                // Validate Chair
                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
                if (string.IsNullOrEmpty(receiver)
                    || string.IsNullOrWhiteSpace(receiver))
                    return false;
            }

            // Get New Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Run Security
            PhoneSecurity.OnCallDecline(phone, caller, receiver);
            PhoneSecurity.OnCallDecline(phone, receiver, caller);

            // Prevent Original
            return false;
        }
    }
}
