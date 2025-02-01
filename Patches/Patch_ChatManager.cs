using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ChatManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0, 
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            ChatManager manager = __0.TryCast<ChatManager>();
            NetworkIdentity sender = __2.identity;

            __1.SafeReadNetworkBehaviour<LobbyPlayer>();

            string message = __1.SafeReadString();
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            message = message.RemoveRichText();
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            int maxCharacters = ConfigHandler.Gameplay.MaxCharactersOnChatMessages.Value;
            if (message.Length > maxCharacters)
                message = message.Substring(0, maxCharacters);
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            LobbyPlayer player = sender.GetLobbyPlayer();
            if (player == null)
                return false;

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__LobbyPlayer__String__NetworkConnectionToClient(player, message, __2);

            // Prevent Original
            return false;
        }
    }
}