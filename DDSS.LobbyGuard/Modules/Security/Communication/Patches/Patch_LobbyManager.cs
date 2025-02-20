using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using System;

namespace DDSS_LobbyGuard.Modules.Security.Communication.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.InvokeUserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get LobbyManager
            LobbyManager manager = __0.TryCast<LobbyManager>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Message and Enforce Timestamp
            __1.SafeReadNetworkIdentity();
            string message = __1.SafeReadString();
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            if (message.Length > 50)
                message = message.Substring(0, 50);
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            // Parse DateTime
            string time = __1.SafeReadString();
            if (string.IsNullOrEmpty(time)
                || string.IsNullOrWhiteSpace(time)
                || !DateTime.TryParse(time, out _))
                return false;

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient(sender, message, time, __2);

            // Prevent Original
            return false;
        }
    }
}
