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
                || (collectible.GetIl2CppType() != Il2CppType.Of<StickyNoteController>()))
                return false;

            // Get StickyNoteController
            StickyNoteController controller = collectible.TryCast<StickyNoteController>();
            if (controller == null)
                return false;

            // Get Value
            string text = __1.ReadString();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Run Game Command
            controller.UserCode_CmdSetText__String(text);

            // Prevent Original
            return false;
        }
    }
}