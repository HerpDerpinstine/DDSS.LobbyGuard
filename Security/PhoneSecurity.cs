using Il2Cpp;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Security
{
    internal static class PhoneSecurity
    {
        private static Dictionary<string, string> _callSenderList = new();
        private static Dictionary<string, string> _callReceiverList = new();

        internal static void OnSceneLoad()
        {
            _callSenderList.Clear();
            _callReceiverList.Clear();
        }

        internal static void OnCallAttempt(PhoneManager phone,
            string caller,
            string receiver)
        {
            // Check if already has Ongoing Call
            if (_callSenderList.ContainsKey(caller))
            {
                // Get Ongoing Receiver
                string oldReceiver = _callSenderList[caller];

                // Check if Call has gone through or not
                if (_callReceiverList.ContainsKey(oldReceiver))
                    phone.UserCode_CmdEndCall__String__String(oldReceiver, caller);
                else
                    phone.UserCode_CmdCancelCall__String__String(caller, oldReceiver);
            }

            // Cache New Attempt
            _callSenderList[caller] = receiver;
        }

        internal static void OnCallAnswer(PhoneManager phone,
            string caller,
            string receiver)
        {
            // Check if Calling Self
            if (caller == receiver)
                return;

            // Check if already has Ongoing Call
            if (_callReceiverList.ContainsKey(receiver))
            {
                // Get Ongoing Receiver
                string oldCaller = _callReceiverList[receiver];

                // Check if Call has gone through or not
                if (_callSenderList.ContainsKey(oldCaller))
                    phone.UserCode_CmdEndCall__String__String(receiver, oldCaller);
                else
                    phone.UserCode_CmdCancelCall__String__String(oldCaller, receiver);
            }

            // Cache New Call
            _callReceiverList[receiver] = caller;

            // Allow Call
            phone.UserCode_CmdAnswerCall__String__String(receiver, caller);
        }

        internal static void OnCallCancel(PhoneManager phone,
            string caller,
            string receiver)
        {
            // Check if Calling Self
            if (caller == receiver)
                return;

            // Check if there is an Ongoing Call Attempt
            if (!_callSenderList.ContainsKey(caller))
                return;

            // Check if there is already an active Call
            if (_callReceiverList.ContainsKey(receiver))
            {
                // Get Ongoing Caller
                string oldCaller = _callReceiverList[receiver];
                if (oldCaller != caller)
                    return;

                // End Call
                phone.UserCode_CmdEndCall__String__String(receiver, caller);
                return;
            }

            // Remove Call from Cache
            _callSenderList.Remove(caller);

            // Cancel Call
            phone.UserCode_CmdCancelCall__String__String(caller, receiver);
        }

        internal static void OnCallEnd(PhoneManager phone,
            string caller,
            string receiver)
        {
            // Check if Calling Self
            if (caller == receiver)
                return;

            // Check if there is an Ongoing Call Attempt
            if (!_callSenderList.ContainsKey(caller))
                return;

            // Check if there is already an active Call
            if (!_callReceiverList.ContainsKey(receiver))
            {
                // End Call
                phone.UserCode_CmdCancelCall__String__String(caller, receiver);
                return;
            }

            // Get Ongoing Caller
            string oldCaller = _callReceiverList[receiver];
            if (oldCaller != caller)
                return;

            // Remove Call from Cache
            _callSenderList.Remove(caller);
            _callReceiverList.Remove(receiver);

            // End Call
            phone.UserCode_CmdEndCall__String__String(receiver, caller);
        }

        internal static void OnCallDecline(PhoneManager phone,
            string caller,
            string receiver)
        {
            // Check if Calling Self
            if (caller == receiver)
                return;

            // Check if already has Ongoing Call
            if (!_callSenderList.ContainsKey(caller))
                return;

            // Remove Call from Cache
            _callSenderList.Remove(caller);

            // Decline Call
            phone.UserCode_CmdDeclineCall__String__String(receiver, caller);
        }
    }
}
