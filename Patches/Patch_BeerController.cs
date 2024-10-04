using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    internal class Patch_BeerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BeerController), nameof(BeerController.InvokeUserCode_CmdSetEmpty))]
        private static bool InvokeUserCode_CmdSetEmpty_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get BeerController
            BeerController beer = __0.TryCast<BeerController>();
            if (beer == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, beer.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<BeerController>()))
                return false;

            // Get Document
            beer = collectible.TryCast<BeerController>();
            if (beer == null)
                return false;

            // Run Game Command
            beer.UserCode_CmdSetEmpty();

            // Prevent Original
            return false;
        }
    }
}