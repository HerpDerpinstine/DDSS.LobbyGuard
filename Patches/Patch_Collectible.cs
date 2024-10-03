using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
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
            // Get Collectible
            Collectible collectible = __0.TryCast<Collectible>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, collectible.transform.position))
                return false;

            // Run Original
            return true;
        }
    }
}