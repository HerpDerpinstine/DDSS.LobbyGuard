using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Printer.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Document
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Document), nameof(Document.InvokeUserCode_CmdSignDocument__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSignDocument__NetworkIdentity__NetworkConnectionToClient_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Get Document
            Document document = __0.TryCast<Document>();
            if ((document == null)
                || document.signed)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible != null)
                && !collectible.WasCollected
                && (collectible.GetIl2CppType() == Il2CppType.Of<Document>()))
            {
                if (collectible == document)
                {
                    // Get Document
                    document = collectible.TryCast<Document>();
                    if ((document == null)
                        || document.signed)
                        return false;
                }
                else if (!InteractionSecurity.IsWithinRange(sender.transform.position, document.transform.position))
                    return false;
            }
            else if (!InteractionSecurity.IsWithinRange(sender.transform.position, document.transform.position))
                return false;

            // Run Game Command
            document.UserCode_CmdSignDocument__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
