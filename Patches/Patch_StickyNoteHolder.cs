using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Scripts;
using Il2CppProps.StickyNote;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_StickyNoteHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteHolder), nameof(StickyNoteHolder.PlaceStickyNote))]
        private static bool PlaceStickyNote_Prefix(StickyNoteHolder __instance,
            NetworkIdentity __0,
            NetworkConnectionToClient __1)
        {
            // Check for Server
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

            // Validate Player State
            if (!ConfigHandler.Gameplay.StickyNotesOnPlayers.Value)
                return false;

            // Place the Sticky Note on the Door
            __instance.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient(sender, __1);

            // Prevent Original
            return false;
        }
    }
}
