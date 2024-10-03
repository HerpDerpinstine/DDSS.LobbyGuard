using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdSetLockState__Boolean))]
        private static bool InvokeUserCode_CmdSetLockState__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();

            // Get Requested Lock State
            bool requestedState = __1.ReadBool();
            int stateIndex = requestedState ? 0 : door.state;

            // Validate Lock
            if ((door.isLocked && (stateIndex != 0))
                || ((door.state == 1) && (stateIndex == -1)) 
                || ((door.state == -1) && stateIndex == 1))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdSetLockState__Boolean(requestedState);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdSetDoorState__Int32))]
        private static bool InvokeUserCode_CmdSetDoorState__Int32_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();

            // Get Requested Lock State
            int stateIndex = __1.ReadInt();

            // Validate Lock
            if ((door.isLocked && (stateIndex != 0))
                || ((door.state == 1) && (stateIndex == -1))
                || ((door.state == -1) && stateIndex == 1))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdSetDoorState__Int32(stateIndex);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdKnockDoor))]
        private static bool InvokeUserCode_CmdKnockDoor_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdKnockDoor();

            // Prevent Original
            return false;
        }
    }
}
