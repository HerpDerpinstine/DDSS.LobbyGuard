using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    internal class Patch_Document
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Document), nameof(Document.InvokeUserCode_CmdSignDocument__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSignDocument__NetworkIdentity_Prefix(
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<Document>()))
                return false;

            // Get Document
            Document document = collectible.TryCast<Document>();
            if (document == null)
                return false;

            // Get Value
            string text = __1.ReadString();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Run Game Command
            document.UserCode_CmdSignDocument__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}