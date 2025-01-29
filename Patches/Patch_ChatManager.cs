﻿using DDSS_LobbyGuard.Security;
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
        [HarmonyPatch(typeof(ChatManager), nameof(ChatManager.InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0, 
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            ChatManager manager = __0.TryCast<ChatManager>();
            NetworkIdentity sender = __2.identity;

            __1.SafeReadNetworkBehaviour<LobbyPlayer>();

            string room = __1.SafeReadString();
            if (string.IsNullOrEmpty(room)
                || string.IsNullOrWhiteSpace(room))
                return false;

            room = room.RemoveRichText();
            if (string.IsNullOrEmpty(room)
                || string.IsNullOrWhiteSpace(room))
                return false;

            string message = __1.SafeReadString();
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            message = message.RemoveRichText();
            if (message.Length > InteractionSecurity.MAX_INGAME_CHAT_CHARS)
                message = message.Substring(0, InteractionSecurity.MAX_INGAME_CHAT_CHARS);
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            LobbyPlayer player = sender.GetLobbyPlayer();
            if (player == null)
                return false;

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__LobbyPlayer__String__String__NetworkConnectionToClient(player, room, message, __2);

            // Prevent Original
            return false;
        }
    }
}