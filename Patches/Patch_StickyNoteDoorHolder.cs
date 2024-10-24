using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;
using Il2CppProps.StickyNote;
using UnityEngine;

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
            // Check for Server
            if ((__1 == null)
                || __1.WasCollected)
                __1 = __0.connectionToClient;
            if (!__1.identity.isServer)
                return false;

            // Get Sender
            NetworkIdentity sender = __1.identity;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Validate Count
            int freeSlots = __instance.freePositions.Count;
            if (!__instance.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, __instance.transform.position))
                return false;

            // Place the Sticky Note on the Door
            __instance.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient(sender, __1);

            // Reset Networked Scale to 1,1,1
            collectible._networkRigidbodyUnreliable.interpolateScale = false;
            collectible._networkRigidbodyUnreliable.syncScale = true;
            collectible._networkRigidbodyUnreliable.SetScale(Vector3.one);

            // Prevent Original
            return false;
        }
    }
}
