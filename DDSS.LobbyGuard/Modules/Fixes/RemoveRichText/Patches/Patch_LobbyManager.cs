using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.Fixes.RemoveRichText.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static void UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
            ref string __1, // message
            ref string __2) // time
        {
            __1 = __1.RemoveRichText();
            __2 = __2.RemoveRichText();
        }
    }
}