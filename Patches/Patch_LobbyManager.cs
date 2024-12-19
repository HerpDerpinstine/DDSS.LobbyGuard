using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using System;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Awake))]
        private static void Awake_Postfix(LobbyManager __instance)
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return;

            // Update Blacklist
            BlacklistSecurity.OnLobbyOpen(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UpdateSettings))]
        private static void UpdateSettings_Postfix()
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return;

            // Update Our Game Settings
            InteractionSecurity.UpdateSettings();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.KickPlayer))]
        private static bool KickPlayer_Prefix(LobbyManager __instance, 
            NetworkIdentity __0)
        {
            // Validate Player
            if ((__0 == null)
                || __0.WasCollected
                || __0.isLocalPlayer)
                return false;

            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Kick Player
            BlacklistSecurity.RequestKick(__0.GetComponent<LobbyPlayer>());

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.BlackListPlayer))]
        private static bool BlackListPlayer_Prefix(LobbyManager __instance,
            LobbyPlayer __0)
        {
            // Validate Player
            if ((__0 == null)
                || __0.WasCollected
                || __0.isLocalPlayer)
                return false;

            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Blacklist Player
            BlacklistSecurity.RequestBlacklist(__0);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UnRegisterPlayer))]
        private static void UnRegisterPlayer_Prefix(LobbyManager __instance,
            NetworkIdentity __0)
        {
            // Validate Server
            if (!__instance.isServer
                || !NetworkServer.activeHost)
                return;

            // Get Lobby Player
            LobbyPlayer lobbyPlayer = __0.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return;

            // Validate Game State
            if (__instance.gameStarted
                && !ConfigHandler.Gameplay.PlayerLeavesReduceTerminations.Value)
            {
                // Adjust Termination Offset
                PlayerRole playerRole = lobbyPlayer.NetworkplayerRole;
                if ((playerRole != PlayerRole.Manager)
                    && (playerRole != PlayerRole.Janitor)
                    && (GameManager.instance != null)
                    && !GameManager.instance.WasCollected)
                {
                    // Get Original Count
                    int slackerCount = GameManager.instance.NetworkstartSlackers;
                    int specialistCount = GameManager.instance.NetworkstartSpecialists;

                    // Get Player Role
                    if (InteractionSecurity.IsSlacker(lobbyPlayer))
                        slackerCount--;
                    else if (playerRole == PlayerRole.Specialist)
                        specialistCount--;

                    // Clamp Count
                    if (slackerCount < 0)
                        slackerCount = 0;
                    if (specialistCount < 0)
                        specialistCount = 0;

                    // Apply New Counts
                    //GameManager.instance.SetWinCondition(specialistCount, slackerCount);
                    GameManager.instance.NetworkstartSlackers = slackerCount;
                    GameManager.instance.NetworkstartSpecialists = specialistCount;
                }
            }

            // Remove Steam ID
            LobbyPlayer localPlayer = __instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer.steamID != lobbyPlayer.steamID))
                LobbySecurity.RemoveValidSteamID(lobbyPlayer.steamID);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.InvokeUserCode_CmdForceManagerRole__NetworkIdentity))]
        private static bool InvokeUserCode_CmdForceManagerRole__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.isServer)
                return true;

            // Check for Lobby
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "LobbyScene")
                return false;

            // Check Sender
            LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
            if ((player == null)
                || player.WasCollected
                || !player.isHost)
                return false;

            // Get LobbyManager
            LobbyManager manager = __0.TryCast<LobbyManager>();

            // Get Target
            NetworkIdentity netIdentity = __1.SafeReadNetworkIdentity();
            if ((netIdentity == null)
                || netIdentity.WasCollected)
                return false;

            // Validate Manager Role
            LobbyPlayer target = netIdentity.GetComponent<LobbyPlayer>();
            if ((target == null)
                || target.WasCollected
                || (target.NetworkplayerRole == PlayerRole.Manager))
                return false;

            // Invoke Game Method
            manager.UserCode_CmdForceManagerRole__NetworkIdentity(netIdentity);

            // Prevent Original
            return false;
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
            __1.SafeReadNetworkIdentity();
            string message = __1.SafeReadString();

            // Validate Message Text
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

            // Invoke Game Method
            manager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String(
                sender, 
                message, 
                DateTime.Now.ToString("HH:mm:ss"));

            // Prevent Original
            return false;
        }
    }
}