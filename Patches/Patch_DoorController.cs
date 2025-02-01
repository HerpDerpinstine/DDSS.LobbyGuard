using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer;
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
        private static void Start_Prefix()
        {
            // Start Door
            DoorSecurity.DoorStart();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient))]
        private static bool UserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient_Prefix(
            DoorController __instance,
            int __0)
        {
            __0 = Mathf.Clamp(__0, -1, 1);

            // Check for Lock
            if ((__0 == 0)
                || __instance.NetworkisLocked)
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
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient_Prefix(
             NetworkBehaviour __0,
             NetworkReader __1,
             NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();
            if ((door == null)
                || door.WasCollected)
                return false;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Get Requested Lock State
            int stateIndex = Mathf.Clamp(__1.SafeReadInt(), -1, 1);

            // Check for Lock
            if ((stateIndex == 0)
                || door.NetworkisLocked)
                return false;

            // Check if already Open
            if (door.Networkstate != 0)
                return false;

            // Apply State
            DoorSecurity.ApplyState(door, stateIndex);

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
            if ((door == null)
                || door.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Get State
            __1.SafeReadNetworkIdentity();
            bool requestedState = __1.SafeReadBool();
            if (door.NetworkisLocked == requestedState)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Validate Lock Request
            if (!door.PlayerCanChangeLockState(sender))
                return false;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Get Player
            LobbyPlayer oldPlayer = controller.NetworklobbyPlayer;
            if ((oldPlayer == null)
                || oldPlayer.WasCollected
                || oldPlayer.IsGhost())
                return false;

            // Get DoorInteractable
            DoorInteractable doorInteractable = door.GetComponentInChildren<DoorInteractable>();
            if ((doorInteractable == null)
                || doorInteractable.WasCollected)
                return false;

            // Validate Role
            var role = oldPlayer.playerRole;
            if (!doorInteractable.everyoneCanLock
                && (role != PlayerRole.Manager))
            {
                if (role == PlayerRole.Janitor)
                {
                    if (requestedState)
                    {
                        if (!ConfigHandler.Gameplay.AllowJanitorsToLockDoors.Value)
                            return false;
                    }
                    else
                    {
                        if (!ConfigHandler.Gameplay.AllowJanitorsToUnlockDoors.Value)
                            return false;
                    }
                }
                else
                {
                    // Validate Placement
                    Collectible collectible = sender.GetCurrentCollectible();
                    if ((collectible == null)
                        || collectible.WasCollected
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
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdKnockDoor__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
