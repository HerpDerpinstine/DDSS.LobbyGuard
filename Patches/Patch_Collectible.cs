using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.PaperShredder;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_Collectible
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Collectible), nameof(Collectible.InvokeUserCode_CmdDestroyCollectible))]
        private static bool InvokeUserCode_CmdDestroyCollectible_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Collectible
            Collectible collectible = __0.TryCast<Collectible>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, collectible.transform.position))
                return false;

            // Validate Held Collectible
            Collectible heldCollectible = sender.GetCurrentCollectible();
            if ((heldCollectible != null)
               && (heldCollectible == collectible)
               && (heldCollectible.GetIl2CppType() == Il2CppType.Of<PaperReam>()))
            {
                // Destroy It
                heldCollectible.UserCode_CmdDestroyCollectible();

                // Prevent Original
                return false;
            }

            // Validate Collectible
            if ((collectible.currentHolder != null)
                && (collectible.currentHolder.GetIl2CppType() == Il2CppType.Of<PaperShredder>()))
            {
                // Destroy It
                collectible.UserCode_CmdDestroyCollectible();
            }

            // Prevent Original
            return false;
        }
    }
}