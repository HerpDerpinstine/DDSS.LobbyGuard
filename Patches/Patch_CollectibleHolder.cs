using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_CollectibleHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity_Prefix(
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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Validate Placement
            if (!InteractionSecurity.IsHoldingCollectible(sender))
                return false;

            // Run Game Command
            holder.UserCode_CmdPlaceCollectibleFromPlayer__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CollectibleHolder), nameof(CollectibleHolder.InvokeUserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity))]
        private static bool InvokeUserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity_Prefix(
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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                return false;

            // Get Collectible NetworkIdentity
            __1.ReadNetworkIdentity();
            NetworkIdentity collectibleIdentity = __1.ReadNetworkIdentity();

            // Get Collectible
            Collectible collectible = collectibleIdentity.GetComponent<Collectible>();
            if (collectible == null)
                return false;
                
            // Validate Grab
            if (!InteractionSecurity.CanGrabCollectible(sender, collectible))
                return false;

            // Run Game Command
            holder.UserCode_CmdGrabCollectible__NetworkIdentity__NetworkIdentity(sender, collectibleIdentity);

            // Prevent Original
            return false;
        }
    }
}
