using HarmonyLib;
using Il2Cpp;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.ServerTimeInMessages.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static void UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(ref string __2)
        {
            if (!ModuleConfig.Instance.UseServerTimeStampForLobbyChatMessages.Value)
                __2 = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}