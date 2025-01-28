using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using System;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ChatManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0, 
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get ChatManager
            ChatManager manager = __0.TryCast<ChatManager>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Message and Enforce Timestamp
            __1.SafeReadNetworkIdentity();
            string message = __1.SafeReadString();
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            // Remove Rich Text
            message = message.RemoveRichText();
            if (message.Length > InteractionSecurity.MAX_CHAT_CHARS)
                message = message.Substring(0, InteractionSecurity.MAX_CHAT_CHARS);
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            // Parse DateTime
            string time = null;
            if (ConfigHandler.General.UseServerTimeStampForChatMessages.Value)
                time = DateTime.Now.ToString("HH:mm:ss");
            else
            {
                time = __1.SafeReadString();
                if (string.IsNullOrEmpty(time)
                    || string.IsNullOrWhiteSpace(time)
                    || !DateTime.TryParse(time, out _))
                    return false;

                time = time.RemoveRichText();
                if (string.IsNullOrEmpty(time)
                    || string.IsNullOrWhiteSpace(time)
                    || !DateTime.TryParse(time, out _))
                    return false;
            }

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient(sender.GetLobbyPlayer(), message, time, __2);

            // Prevent Original
            return false;
        }
    }
}