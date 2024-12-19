using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using Il2CppProps.ServerRack;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_EmailManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EmailManager), nameof(EmailManager.RegisterEmail))]
        private static void RegisterEmail_Prefix(string __0, Color __1)
        {
            // Add Player Email to Cache
            __0 = __0.ToLower();
            ComputerSecurity._playerAddresses[__0.ToLower()] = __1;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(EmailManager), nameof(EmailManager.InvokeUserCode_CmdSendEmail__String__String__String__String__String))]
        private static bool InvokeUserCode_CmdSendEmail__String__String__String__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check if Servers are Working
            if (!ServerController.connectionsEnabled)
                return false;

            // Get EmailManager
            EmailManager inbox = __0.TryCast<EmailManager>();
            if ((inbox == null)
                || inbox.WasCollected)
                return false;

            // Get and Ignore User Input Sender
            string sender = __1.SafeReadString();
            if (string.IsNullOrEmpty(sender)
                || string.IsNullOrWhiteSpace(sender))
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

                // Enforce Sender Address
                sender = workStation.computerController.emailAddress;
            }

            // Validate Sender
            string senderLower = sender.ToLower();
            bool isPlayer = ComputerSecurity._playerAddresses.ContainsKey(senderLower);
            bool isClient = Task.clientEmails.Contains(senderLower);
            if (!isPlayer && !isClient)
                return false;

            // Get and Ignore User Input Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Validate Receiver
            string recipientLower = receiver.ToLower();
            isPlayer = ComputerSecurity._playerAddresses.ContainsKey(recipientLower);
            isClient = Task.clientEmails.Contains(recipientLower);
            if (!isPlayer && !isClient)
                return false;

            // Get Subject
            string subject = __1.SafeReadString();
            if (string.IsNullOrEmpty(subject)
                || string.IsNullOrWhiteSpace(subject))
                return false;

            // Get Message
            string msg = __1.SafeReadString();
            if (string.IsNullOrEmpty(msg)
                || string.IsNullOrWhiteSpace(msg))
                return false;

            // Enforce Timestamp
            string timeStamp = DateTime.Now.ToString("HH:mm");

            // Run Game Command
            inbox.UserCode_CmdSendEmail__String__String__String__String__String(sender, receiver, subject, msg, timeStamp);

            // Prevent Original
            return false;
        }
    }
}