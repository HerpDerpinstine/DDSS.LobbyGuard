using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_CollectibleHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdPlaceCollectible__NetworkIdentity__String))]
        private static bool InvokeUserCode_CmdPlaceCollectible__NetworkIdentity__String_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get CollectibleHolder
            CollectibleHolder holder = __0.TryCast<CollectibleHolder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Holder
            if (!CollectibleHolderSecurity.CanPlace(holder, collectible))
                return false;

            // Validate Free Slots
            int freeSlots = holder.freePositions.Count;
            if (!holder.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = holder.collectibles.Count;
            if (holder.allowStacking && (usedSlots >= InteractionSecurity.MAX_COLLECTIBLES_HOLDER))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Run Game Command
            holder.UserCode_CmdPlaceCollectible__NetworkIdentity__String(collectible.netIdentity,
                collectible.label);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get CollectibleHolder
            CollectibleHolder holder = __0.TryCast<CollectibleHolder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Free Slots
            int freeSlots = holder.freePositions.Count;
            if (!holder.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = holder.collectibles.Count;
            if (holder.allowStacking && (usedSlots >= InteractionSecurity.MAX_COLLECTIBLES_HOLDER))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Holder
            if (!CollectibleHolderSecurity.CanPlace(holder, collectible))
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
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get CollectibleHolder
            CollectibleHolder holder = __0.TryCast<CollectibleHolder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Count
            if (holder.freePositions.Count >= holder.collectiblePositions.Count)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Get Collectible NetworkIdentity
            __1.SafeReadNetworkIdentity();
            NetworkIdentity collectibleIdentity = __1.SafeReadNetworkIdentity();

            // Get Collectible
            Collectible collectible = collectibleIdentity.GetComponent<Collectible>();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Collectible
            if (collectible.currentHolder != holder)
                return false;

            // Run Game Command
            holder.UserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient(sender, collectibleIdentity, __2);

            // Prevent Original
            return false;
        }
    }
}
