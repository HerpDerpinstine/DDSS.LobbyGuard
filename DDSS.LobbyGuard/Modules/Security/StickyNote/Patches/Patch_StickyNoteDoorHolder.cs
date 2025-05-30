﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Door;
using Il2CppProps.Scripts;
using Il2CppProps.StickyNote;

namespace DDSS_LobbyGuard.Modules.Security.StickyNote.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_StickyNoteDoorHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteDoorHolder), nameof(StickyNoteDoorHolder.PlaceStickyNote))]
        private static bool PlaceStickyNote_Prefix(StickyNoteDoorHolder __instance,
            NetworkIdentity __0,
            NetworkConnectionToClient __1)
        {
            if (__instance == null
                || __instance.WasCollected)
                return false;

            if (!__instance.isServer)
                return true;

            if (!NetworkServer.activeHost)
                return true;

            if (__0 == null
                || __0.WasCollected)
                return false;

            // Get NetworkConnectionToClient
            if (__1 == null
                || __1.WasCollected)
                __1 = __0.connectionToClient;

            // Validate Placement
            Collectible collectible = __0.GetCurrentCollectible();
            if (collectible == null
                || collectible.WasCollected)
                return false;

            // Get StickyNoteController
            StickyNoteController stickyNote = collectible.TryCast<StickyNoteController>();
            if (stickyNote == null
                || stickyNote.WasCollected)
                return false;

            // Validate Free Slots
            int freeSlots = __instance.freePositions.Count;
            if (!__instance.allowStacking && freeSlots <= 0)
                return false;

            // Validate Used Slots
            int usedSlots = __instance.collectibles.Count;
            if (__instance.allowStacking && usedSlots >= InteractionSecurity.MAX_STICKYNOTES_DOOR)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(__0, __instance))
                return false;

            // Get DoorInteractable
            DoorInteractable doorInteractable = __instance.parentInteractable.TryCast<DoorInteractable>();
            if (doorInteractable == null
                || doorInteractable.WasCollected)
                return false;

            // Get DoorController
            DoorController door = doorInteractable.doorController;
            if (door == null
                || door.WasCollected)
                return false;

            // Validate Door State
            if (!ModuleConfig.Instance.StickyNotesOnDoors.Value)
                return false;

            // Place the Sticky Note on the Door
            __instance.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient(__0, __1);

            // Prevent Original
            return false;
        }
    }
}
