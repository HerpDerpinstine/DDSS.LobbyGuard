using HarmonyLib;
using Il2Cpp;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.ServerTimeInMessages.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_EmailManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EmailManager), nameof(EmailManager.UserCode_CmdSendEmail__String__String__String__String__String))]
        private static void UserCode_CmdSendEmail__String__String__String__String__String_Prefix(ref string __4)
        {
            if (!ModuleConfig.Instance.UseServerTimeStampForChatMessages.Value)
                return;
            __4 = DateTime.Now.ToString("HH:mm");
        }
    }
}