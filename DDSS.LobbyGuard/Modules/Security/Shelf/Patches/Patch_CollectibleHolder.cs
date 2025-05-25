using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Modules.Security.Shelf.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CollectibleHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.ServerDestroyAllCollectibles))]
        private static bool ServerDestroyAllCollectibles_Prefix(CollectibleHolder __instance)
        {
            if ((__instance == null)
                || __instance.WasCollected
                || (__instance.collectibles == null)
                || __instance.collectibles.WasCollected
                || (__instance.collectibles.Count <= 0))
                return false;

            List<Il2CppProps.Scripts.Collectible> list = new();
            foreach (var item in __instance.collectibles)
            {
                if ((item == null)
                    || item.WasCollected)
                    continue;

                Il2CppProps.Scripts.Collectible collectible = item.Key;
                if ((collectible == null)
                    || collectible.WasCollected)
                    continue;

                list.Add(collectible);
            }
            foreach (var item in list)
                item.ServerDestroyCollectible();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CollectibleHolder
            CollectibleHolder holder = __0.TryCast<CollectibleHolder>();
            if ((holder == null)
                || holder.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Free Slots
            int freeSlots = holder.freePositions.Count;
            if (!holder.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = holder.collectibles.Count;
            if (holder.allowStacking
                && (usedSlots >= InteractionSecurity.MAX_COLLECTIBLES_HOLDER))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Validate Placement
            Il2CppProps.Scripts.Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Holder
            if (!CollectibleSecurity.CanPlace(sender, holder, collectible))
                return false;

            // Run Game Command
            holder.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get CollectibleHolder
            CollectibleHolder holder = __0.TryCast<CollectibleHolder>();
            if ((holder == null)
                || holder.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Get Collectible NetworkIdentity
            __1.SafeReadNetworkIdentity();
            NetworkIdentity collectibleIdentity = __1.SafeReadNetworkIdentity();
            if ((collectibleIdentity == null)
                || collectibleIdentity.WasCollected)
                return false;

            // Get Collectible
            Il2CppProps.Scripts.Collectible collectible = collectibleIdentity.GetComponent<Il2CppProps.Scripts.Collectible>();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Collectible
            if (collectible.currentHolder != holder)
                return false;

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, collectible))
                return false;

            // Run Game Command
            holder.UserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient(sender, collectibleIdentity, __2);

            // Prevent Original
            return false;
        }
    }
}