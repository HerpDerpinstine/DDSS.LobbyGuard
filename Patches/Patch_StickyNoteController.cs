using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    [HarmonyPatch]
    internal class Patch_StickyNoteController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteController), nameof(StickyNoteController.InvokeUserCode_CmdSetText__String__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetText__String__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Get StickyNoteController
            StickyNoteController __instance = __0.TryCast<StickyNoteController>();
            if (__instance == null)
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<StickyNoteController>()))
                return false;

            // Get StickyNoteController
            __instance = collectible.TryCast<StickyNoteController>();
            if (__instance == null)
                return false;

            // Check if Sticky Note is already Written To
            if (__instance.hasBeenEdited)
                return false;

            // Validate Message Text
            string text = __1.SafeReadString();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Remove Rich Text
            text = text.RemoveRichText();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            int maxCharacters = ConfigHandler.Gameplay.MaxCharactersOnStickyNotes.Value;
            if (text.Length > maxCharacters)
                text = text.Substring(0, maxCharacters);
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Run Game Command
            __instance.UserCode_CmdSetText__String__NetworkIdentity__NetworkConnectionToClient(text, sender, __2);

            // Change Name
            eConfigHostType configValue = ConfigHandler.Gameplay.UsernamesOnStickyNotes.Value;
            if (configValue != eConfigHostType.DISABLED)
            {
                // Apply New Name
                string userName = sender.GetUserName();
                if (!string.IsNullOrEmpty(userName)
                    && !string.IsNullOrWhiteSpace(userName))
                {
                    string newName = $"{userName.RemoveRichText()}'s Sticky Note";
                    if (configValue == eConfigHostType.HOST_ONLY)
                        collectible.interactableName =
                            collectible.label = newName;
                    else
                        collectible.NetworkinteractableName =
                            collectible.Networklabel = newName;
                }
            }

            __instance.UpdateCollectible();

            // Prevent Original
            return false;
        }
    }
}