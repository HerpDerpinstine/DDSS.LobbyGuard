﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using System;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_LobbyManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UpdateSettings))]
        private static void UpdateSettings_Postfix()
        {
            // Update Our Game Settings
            InteractionSecurity.UpdateSettings();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UnRegisterPlayer))]
        private static void UnRegisterPlayer_Prefix(LobbyManager __instance,
            NetworkIdentity player)
        {
            // Validate Server
            if (!__instance.isServer)
                return;

            // Get PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
                return;

            // Get Usable
            Usable usable = controller.GetCurrentUsable();
            if (usable == null)
                return;

            // Drop It
            usable.ServerStopUse(player);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.InvokeUserCode_CmdForceManagerRole__NetworkIdentity))]
        private static bool InvokeUserCode_CmdForceManagerRole__NetworkIdentity_Prefix(NetworkConnectionToClient __2)
        {
            // Validate
            if ((__2 == null)
                || __2.WasCollected)
                return false;

            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Check for Lobby
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyScene")
                return false;

            // Validate Manager Role when In-Game
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.InvokeUserCode_CmdSendChatMessage__NetworkIdentity__String__String))]
        private static bool InvokeUserCode_CmdSendChatMessage__NetworkIdentity__String__String_Prefix(
            NetworkBehaviour __0, 
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get LobbyManager
            LobbyManager manager = __0.TryCast<LobbyManager>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Message and Enforce Timestamp
            __1.ReadNetworkIdentity();
            string message = __1.ReadString();

            // Validate Message Text
            if (string.IsNullOrEmpty(message)
                || string.IsNullOrWhiteSpace(message))
                return false;

            // Remove Rich Text
            message = message.RemoveRichText();

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String(sender, message, DateTime.Now.ToString("HH:mm:ss"));

            // Prevent Original
            return false;
        }
    }
}