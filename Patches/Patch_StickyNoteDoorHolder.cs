using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Door;
using Il2CppProps.Scripts;
using Il2CppProps.StickyNote;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_StickyNoteDoorHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteDoorHolder), nameof(StickyNoteDoorHolder.PlaceStickyNote))]
        private static bool PlaceStickyNote_Prefix(StickyNoteDoorHolder __instance,
            NetworkIdentity __0,
            NetworkConnectionToClient __1)
        {
            // Get NetworkConnectionToClient
            if ((__1 == null)
                || __1.WasCollected)
                __1 = __0.connectionToClient;

            // Get Sender
            NetworkIdentity sender = __1.identity;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get StickyNoteController
            StickyNoteController stickyNote = collectible.TryCast<StickyNoteController>();
            if (stickyNote == null)
                return false;

            // Validate Count
            int freeSlots = __instance.freePositions.Count;
            if (!__instance.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, __instance.transform.position))
                return false;

            // Get DoorInteractable
            DoorInteractable doorInteractable = __instance.parentInteractable.TryCast<DoorInteractable>();
            if ((doorInteractable == null)
                || doorInteractable.WasCollected)
                return false;

            // Get DoorController
            DoorController door = doorInteractable.doorController;
            if ((door == null)
                || door.WasCollected)
                return false;

            // Validate Door State
            if (!ConfigHandler.Gameplay.StickyNotesOnOpenDoors.Value
                && (door.Networkstate != 0))
                return false;
            if (!ConfigHandler.Gameplay.StickyNotesOnClosedDoors.Value
                && (door.Networkstate == 0))
                return false;

            // Place the Sticky Note on the Door
            __instance.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient(sender, __1);

            // Prevent Original
            return false;
        }
    }
}
