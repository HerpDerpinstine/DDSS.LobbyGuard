﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ElevatorDoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ElevatorDoorController), nameof(ElevatorDoorController.InvokeUserCode_CmdRequestOpenDoor))]
        private static bool InvokeUserCode_CmdRequestOpenDoor_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get ElevatorDoorController
            ElevatorDoorController door = __0.TryCast<ElevatorDoorController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !door.enabled
                || !door.NetworkcanOpenDoor
                || door.isDoorOpen
                || !InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdRequestOpenDoor();

            // Prevent Original
            return false;
        }
    }
}
