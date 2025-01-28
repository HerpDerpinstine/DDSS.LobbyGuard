﻿using DDSS_LobbyGuard.Utils;
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
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdAnswerCall__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdAnswerCall__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
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

            // Run Game Command
            phone.ForceCallToEnd(sender, __2);
            phoneManager.UserCode_CmdAnswerCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, receiver, caller, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdCancelCall__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCancelCall__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
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

            // Run Game Command
            phone.ForceCallToEnd(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdEndCall__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEndCall__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
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

            // Run Game Command
            phone.ForceCallToEnd(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.InvokeUserCode_CmdDeclineCall__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdDeclineCall__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
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

            // Run Game Command
            phoneManager.UserCode_CmdDeclineCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, caller, receiver, __2);

            // Prevent Original
            return false;
        }
    }
}
