using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppUI.Tabs.PlayerManagementTab;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerListAlternatives
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerListAlternatives), nameof(PlayerListAlternatives.BuildAlternatives))]
        private static bool UpdateTab_Prefix(PlayerListAlternatives __instance)
        {
            // Destroy Existing Buttons
            int childCount = __instance.gridLayoutGroup.transform.childCount;
            for (int i = 0; i < childCount; i++)
                GameObject.Destroy(__instance.gridLayoutGroup.transform.GetChild(i).gameObject);
            __instance.childCount = 0;

            // Create Toggle Manager Button
            bool areYouHosting = NetworkServer.activeHost;

            // Create View Profile Button
            bool isThisMe = __instance.playerLobbyUI.lobbyPlayer.isMine;
            if (!isThisMe)
                __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("View Profile"), new Action(__instance.playerLobbyUI.ViewProfile));

            // Check for Host and Other Players
            if (areYouHosting && !isThisMe)
            {
                // Create Mute Button
                if (__instance.playerLobbyUI.lobbyPlayer.IsMuted())
                    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Unmute"), new Action(__instance.playerLobbyUI.Unmute));
                else
                    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Mute"), new Action(__instance.playerLobbyUI.Mute));

                // Create Kick Button
                __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Kick"), new Action(__instance.playerLobbyUI.KickPlayer));

                // Create Blacklist Button
                __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Blacklist"), new Action(__instance.playerLobbyUI.BlacklistPlayer));
            }

            // Fix Panel Size
            __instance.backgroundImage.sizeDelta = new Vector2(
                __instance.backgroundImage.sizeDelta.x, 
                86.2f + (__instance.childCount - 1) * 34.7f);

            // Prevent Original
            return false;
        }
    }
}
