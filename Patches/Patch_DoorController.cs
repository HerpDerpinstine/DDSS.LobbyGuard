using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Door;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.Start))]
        private static void Start_Prefix(DoorController __instance)
        {
            if ((__instance == null)
                || __instance.WasCollected)
                return;

            // Fix Colliders
            DoorSecurity.FixColliderSize(__instance.playerDetectionVolumeForward);
            DoorSecurity.FixColliderSize(__instance.playerDetectionVolumeBackward);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetDoorState__Int32))]
        private static bool UserCode_CmdSetDoorState__Int32_Prefix(
            DoorController __instance,
            int __0)
        {
            // Check for Open Request
            if (__0 == 0)
                return false;

            // Check for Lock
            if (__instance.isLocked
                && (__0 != 0))
                return false;

            // Check if already Open
            if (__instance.Networkstate != 0)
                return false;

            // Apply State
            DoorSecurity.ApplyState(__instance, __0);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get State
            __1.ReadNetworkIdentity();
            bool requestedState = __1.ReadBool();
            if (door.isLocked == requestedState)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Check for Server
            if (!sender.isServer)
            {
                // Get Player
                LobbyPlayer oldPlayer = sender.GetComponent<LobbyPlayer>();
                if (oldPlayer == null)
                    return false;

                // Get DoorInteractable
                DoorInteractable doorInteractable = door.GetComponentInChildren<DoorInteractable>();
                if (doorInteractable == null)
                    return false; 

                // Validate Role
                var role = oldPlayer.NetworkplayerRole;
                if (!doorInteractable.everyoneCanLock
                    && (role != PlayerRole.Manager)
                    && (role != PlayerRole.Janitor))
                {
                    // Validate Placement
                    Collectible collectible = sender.GetCurrentCollectible();
                    if ((collectible == null)
                        || (collectible.GetIl2CppType() != Il2CppType.Of<KeyController>()))
                        return false;
                }
            }

            // Apply State
            door.UserCode_CmdSetLockState__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, requestedState, __2);

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
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdKnockDoor__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdKnockDoor__NetworkIdentity__NetworkConnectionToClient_Prefix(
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
            door.UserCode_CmdKnockDoor__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
