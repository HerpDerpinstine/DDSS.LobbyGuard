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
    internal class Patch_PhoneController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneController), nameof(PhoneController.InvokeUserCode_CmdCall__NetworkIdentity__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCall__NetworkIdentity__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get PhoneController
            PhoneController phone = __0.TryCast<PhoneController>();
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get and Ignore User Input Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
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

            // Validate Chair
            WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.ownerLobbyPlayer == null)
                || workStation.ownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.ownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            caller = phone.phoneNumber;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get New Caller
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Prevent Calling Self
            if (caller == receiver)
                return false;

            // Run Security
            PhoneManager.instance.ServerCall(caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
