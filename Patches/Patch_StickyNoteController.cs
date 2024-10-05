using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    internal class Patch_StickyNoteController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteController), nameof(StickyNoteController.InvokeUserCode_CmdSetText__String))]
        private static bool InvokeUserCode_CmdSetText__String_Prefix(
           NetworkBehaviour __0,
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get StickyNoteController
            StickyNoteController controller = __0.TryCast<StickyNoteController>();
            if (controller == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<StickyNoteController>()))
                return false;

            // Get StickyNoteController
            controller = collectible.TryCast<StickyNoteController>();
            if (controller == null)
                return false;

            // Get Value
            string text = __1.ReadString();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Apply Username to Label
            string username = sender.GetUserName();
            if (!string.IsNullOrEmpty(username)
                && !string.IsNullOrWhiteSpace(username))
                controller.label = $"{username.RemoveRichText()}'s Sticky Note";

            // Run Game Command
            controller.UserCode_CmdSetText__String(text.RemoveRichText());

            // Prevent Original
            return false;
        }
    }
}