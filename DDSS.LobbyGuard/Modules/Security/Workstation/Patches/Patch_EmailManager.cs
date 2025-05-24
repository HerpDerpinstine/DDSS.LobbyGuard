using DDSS_LobbyGuard.Modules.Security.Workstation.Internal;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Workstation.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
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
            // Get EmailManager
            EmailManager inbox = __0.TryCast<EmailManager>();
            if ((inbox == null)
                || inbox.WasCollected)
                return false;

            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            string sender = __1.SafeReadString();
            if (string.IsNullOrEmpty(sender)
                || string.IsNullOrWhiteSpace(sender))
                return false;

            string senderLower = sender.ToLower();
            bool isSenderClient = Task.clientEmails.Contains(senderLower);
            bool isSenderPlayer = !isSenderClient && ComputerSecurity._playerAddresses.ContainsKey(senderLower);
            if (!isSenderPlayer && !isSenderClient)
                return false;

            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(sender)
                || string.IsNullOrWhiteSpace(sender))
                return false;

            string receiverLower = receiver.ToLower();
            bool isReceiverClient = Task.clientEmails.Contains(receiverLower);
            bool isReceiverPlayer = !isReceiverClient && ComputerSecurity._playerAddresses.ContainsKey(receiverLower);
            if (!isReceiverPlayer && !isReceiverClient)
                return false;

            string subject = __1.SafeReadString();
            string msg = __1.SafeReadString();

            string time = __1.SafeReadString();
            if (string.IsNullOrEmpty(time)
                || string.IsNullOrWhiteSpace(time)
                || !DateTime.TryParse(time, out _))
                return false;

            if (isSenderClient)
            {
                // Client -> ???

                if (isReceiverPlayer)
                {
                    // Client -> Player

                    if (!ComputerSecurity.EnforceClientEmailSubject(subject))
                        return false;

                    WorkStationController workStation = lobbyPlayer.NetworkworkStationController;
                    if ((workStation == null)
                        || workStation.WasCollected)
                        return false;

                    receiver = workStation.computerController.emailAddress;
                    receiverLower = receiver.ToLower();
                }
                else
                {
                    // Client -> Client
                    return false; // Do Nothing because Transmission isn't Needed
                }
            }
            else
            {
                // Player -> ???

                if ((controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                sender = workStation.computerController.emailAddress;
                senderLower = sender.ToLower();

                if (isReceiverPlayer)
                {
                    // Player -> Player

                    if (string.IsNullOrEmpty(subject)
                        || string.IsNullOrWhiteSpace(subject)
                        || string.IsNullOrEmpty(msg)
                        || string.IsNullOrWhiteSpace(msg))
                        return false;
                }
                else
                {
                    // Player -> Client
                    return false; // Do Nothing because Trasmission isn't Needed
                }
            }

            // Run Game Command
            inbox.UserCode_CmdSendEmail__String__String__String__String__String(senderLower, receiverLower, subject, msg, time);

            // Prevent Original
            return false;
        }
    }
}