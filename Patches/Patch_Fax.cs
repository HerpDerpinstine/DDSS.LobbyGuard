using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_Fax
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fax), nameof(Fax.InvokeUserCode_CmdFax__NetworkIdentity))]
        private static bool InvokeUserCode_CmdFax__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Fax
            Fax fax = __0.TryCast<Fax>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, fax.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || ((collectible.GetIl2CppType() != Il2CppType.Of<Document>())
                    && (collectible.GetIl2CppType() != Il2CppType.Of<PrintedImage>())))
                return false;

            // Get Document
            Document doc = collectible.TryCast<Document>();
            if (doc == null)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(fax.transform.position, doc.transform.position))
                return false;

            // Run Game Command
            fax.UserCode_CmdFax__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
