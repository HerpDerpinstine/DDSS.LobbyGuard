using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetDoorState__Int32))]
        private static bool UserCode_CmdSetDoorState__Int32_Prefix(
            DoorController __instance,
            int __0)
        {
            // Apply State
            DoorSecurity.ApplyState(__instance, __0);

            // Prevent Original
            return false;
        }

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

            // Get Sender
            NetworkIdentity sender = __2.identity;

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

                // Validate Role
                var role = oldPlayer.NetworkplayerRole;
                if (role != PlayerRole.Manager && role != PlayerRole.Janitor)
                {
                    // Validate Placement
                    Collectible collectible = sender.GetCurrentCollectible();
                    if ((collectible == null)
                        || (collectible.GetIl2CppType() != Il2CppType.Of<KeyController>()))
                        return false;
                }
            }

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
