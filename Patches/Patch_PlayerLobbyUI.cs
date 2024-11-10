using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppTMPro;
using Il2CppUI.Tabs.LobbyTab;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerLobbyUI
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.Start))]
        private static void Start_Postfix(PlayerLobbyUI __instance)
        {
            if (__instance.playerName == null)
                return;

            // Check Config
            if (ConfigHandler.Lobby.CharacterNamesInPlayerList.Value)
            {
                // Clone playerName
                GameObject characterNameObj = GameObject.Instantiate(__instance.playerName.gameObject, __instance.playerName.transform.parent);
                characterNameObj.name = "CharacterName";

                // Set characterName
                TextMeshProUGUI characterName = characterNameObj.GetComponent<TextMeshProUGUI>();
                characterName.text = __instance.lobbyPlayer.Networkusername;
                characterName.color = Color.black;
                InteractionSecurity.AddLobbyUICharacterName(__instance, characterName);

                // Move characterName
                Vector3 localPos = __instance.playerName.transform.localPosition;
                localPos.y = 7;
                characterName.transform.localPosition = localPos;

                // Move playerName
                localPos.y = -7;
                __instance.playerName.transform.localPosition = localPos;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.Update))]
        private static void Update_Postfix(PlayerLobbyUI __instance)
        {
            if (__instance.playerName == null)
                return;

            // Check Config
            if (ConfigHandler.Lobby.CharacterNamesInPlayerList.Value)
            {
                TextMeshProUGUI characterName = InteractionSecurity.GetLobbyUICharacterName(__instance);
                if ((characterName != null)
                    && !characterName.WasCollected)
                    characterName.text = __instance.lobbyPlayer.Networkusername;
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.KickPlayer))]
        private static bool KickPlayer_Prefix(PlayerLobbyUI __instance)
        {
            // Check for Server
            if ((LobbyManager.instance == null)
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if ((player == null)
                || player.WasCollected
                || player.isLocalPlayer)
                return false;

            // Kick Player
            BlacklistSecurity.RequestKick(player);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.BlacklistPlayer))]
        private static bool BlacklistPlayer_Prefix(PlayerLobbyUI __instance)
        {
            // Check for Server
            if ((LobbyManager.instance == null)
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance.isServer)
                return false;

            // Get LobbyPlayer
            LobbyPlayer player = __instance.lobbyPlayer;
            if ((player == null)
                || player.WasCollected
                || player.isLocalPlayer)
                return false;

            // Blacklist Player
            BlacklistSecurity.RequestBlacklist(player);

            // Prevent Original
            return false;
        }
    }
}
