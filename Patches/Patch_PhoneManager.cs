using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_PhoneManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.CmdCall))]
        private static bool CmdCall_Prefix(string __0, string __1)
        {
            // Prevent Calling Self
            if (__0 == __1)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdCall__String__String))]
        private static bool InvokeUserCode_CmdCall__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Phone
            PhoneManager phone = __0.TryCast<PhoneManager>();

            // Get and Ignore User Input Caller
            string caller = __1.ReadString();

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Enforce Caller Number
                caller = phone.GetPhoneNumber(player);
            }

            // Get New Receiver
            string receiver = __1.ReadString();

            // Run Security
            PhoneSecurity.OnCallAttempt(phone, caller, receiver);

            // Prevent Original
            return false;
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

            // Get and Ignore User Input Receiver
            string receiver = __1.ReadString();

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
            }

            // Get New Caller
            string caller = __1.ReadString();

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

            // Get and Ignore User Input Caller
            string caller = __1.ReadString();

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Enforce Caller Number
                caller = phone.GetPhoneNumber(player);
            }

            // Get New Receiver
            string receiver = __1.ReadString();

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

            // Get and Ignore User Input Receiver
            string receiver = __1.ReadString();

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
            }

            // Get New Caller
            string caller = __1.ReadString();

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

            // Get and Ignore User Input Receiver
            string receiver = __1.ReadString();
            
            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if (player == null)
                    return false;

                // Enforce Receiver Number
                receiver = phone.GetPhoneNumber(player);
            }

            // Get New Caller
            string caller = __1.ReadString();

            // Run Security
            PhoneSecurity.OnCallDecline(phone, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
