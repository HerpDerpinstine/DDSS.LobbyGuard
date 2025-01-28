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
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.ServerKickPlayer))]
        private static bool ServerKickPlayer_Prefix(LobbyManager __instance, 
            NetworkIdentity __0)
        {
            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Validate Player
            if ((__0 == null)
                || __0.WasCollected
                || __0.isLocalPlayer)
                return false;

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
            // Check for Host
            if (!NetworkServer.activeHost)
                return true;

            // Validate Player
            if ((__0 == null)
                || __0.WasCollected
                || __0.isLocalPlayer
                || (__0.NetworksteamID == 0))
                return false;

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
                && !lobbyPlayer.IsGhost())
            {
                bool isJanitor = lobbyPlayer.IsJanitor();
                PlayerRole playerRole = lobbyPlayer.NetworkplayerRole;

                // Reset WorkStation
                if (!isJanitor 
                    || ConfigHandler.Gameplay.AllowJanitorsToKeepWorkStation.Value)
                    lobbyPlayer.ServerSetWorkStation(null, playerRole, true);

                if (isJanitor)
                {
                    lobbyPlayer.NetworkisFired = true;
                    lobbyPlayer.isFired = true;
                }

                // Check Setting
                if (!ConfigHandler.Gameplay.PlayerLeavesReduceTerminations.Value
                    && !isJanitor
                    && (playerRole != PlayerRole.Manager)
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

            // Remove Rich Text
            message = message.RemoveRichText();
            if (message.Length > InteractionSecurity.MAX_LOBBY_CHAT_CHARS)
                message = message.Substring(0, InteractionSecurity.MAX_LOBBY_CHAT_CHARS);
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
            manager.UserCode_CmdSendChatMessage__NetworkIdentity__String__String__NetworkConnectionToClient(sender, message, time, __2);

            // Prevent Original
            return false;
        }
    }
}