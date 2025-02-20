using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.Fixes.RemoveRichText.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ChatManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.UserCode_CmdSendChatMessage__LobbyPlayer__String__NetworkConnectionToClient))]
        private static bool UserCode_CmdSendChatMessage__LobbyPlayer__String__NetworkConnectionToClient_Prefix(ref string __1)
        {
            __1 = __1.RemoveRichText();
            if (string.IsNullOrEmpty(__1)
                || string.IsNullOrWhiteSpace(__1))
                return false;

            // Run Original
            return true;
        }
    }
}
