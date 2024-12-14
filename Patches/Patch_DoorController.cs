using DDSS_LobbyGuard.Config;
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

            // Start Door
            DoorSecurity.DoorStart(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.InvokeUserCode_CmdSetDoorState__Int32))]
        private static bool InvokeUserCode_CmdSetDoorState__Int32_Prefix(
             NetworkBehaviour __0,
             NetworkReader __1,
             NetworkConnectionToClient __2)
        {
            // Get Requested Lock State
            int stateIndex = Mathf.Clamp(__1.SafeReadInt(), -1, 1);

            // Apply Server State
            if (__2.identity.isServer)
            {
                if (stateIndex == 0)
                    return ConfigHandler.Gameplay.CloseDoorsOnLock.Value;
                return true;
            }

            // Get DoorController
            DoorController door = __0.TryCast<DoorController>();
            if ((door == null)
                || door.WasCollected)
                return false;

            // Check for Open Request
            if (stateIndex == 0)
                return false;

            // Check for Lock
            if (door.NetworkisLocked
                && (stateIndex != 0))
                return false;

            // Check if already Open
            if (door.Networkstate != 0)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
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

            // Check for Server
            if (!sender.isServer)
            {
                // Get Player
                LobbyPlayer oldPlayer = sender.GetComponent<LobbyPlayer>();
                if ((oldPlayer == null)
                    || oldPlayer.WasCollected)
                    return false;

                // Get DoorInteractable
                DoorInteractable doorInteractable = door.GetComponentInChildren<DoorInteractable>();
                if ((doorInteractable == null)
                    || doorInteractable.WasCollected)
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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, door.transform.position))
                return false;

            // Run Game Command
            door.UserCode_CmdKnockDoor__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
