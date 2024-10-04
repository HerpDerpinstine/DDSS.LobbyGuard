using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_StorageBox
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StorageBox), nameof(StorageBox.InvokeUserCode_CmdGrabObject__NetworkIdentity))]
        private static bool InvokeUserCode_CmdGrabObject__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StorageBox
            StorageBox box = __0.TryCast<StorageBox>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, box.transform.position))
                return false;

            // Get Collectible
            Collectible collectible = box.objectPrefab.GetComponent<Collectible>();
            if (collectible == null)
                return false;

            // Validate Grab
            if (!InteractionSecurity.CanGrabCollectible(sender, collectible))
                return false;

            // Run Game Command
            box.UserCode_CmdGrabObject__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
