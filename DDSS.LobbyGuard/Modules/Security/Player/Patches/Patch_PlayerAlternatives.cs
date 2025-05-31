using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppUI.Tabs.LobbyTab;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Player.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PlayerAlternatives
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerAlternatives), nameof(PlayerAlternatives.UpdateTab))]
        private static bool UpdateTab_Prefix(PlayerAlternatives __instance)
        {
            // Get Lobby UI Reference
            __instance.playerLobbyUI = __instance.GetComponentInParent<PlayerLobbyUI>();

            // Destroy Existing Buttons
            int childCount = __instance.gridLayoutGroup.transform.childCount;
            for (int i = 0; i < childCount; i++)
                GameObject.Destroy(__instance.gridLayoutGroup.transform.GetChild(i).gameObject);
            __instance.childCount = 0;

            // Create Toggle Manager Button
            bool areYouHosting = NetworkServer.activeHost;
            if (areYouHosting)
                __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Toggle Manager"), new Action(__instance.playerLobbyUI.ToggleManager));

            // Create View Profile Button
            __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("View Profile"), new Action(__instance.playerLobbyUI.ViewProfile));

            // Check for Other Players
            bool isThisMe = __instance.playerLobbyUI.lobbyPlayer.isMine;
            if (!isThisMe)
            {
                // Create Mute/Unmute Button
                //if (__instance.playerLobbyUI.lobbyPlayer.IsMuted())
                //    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Unmute"), new Action(() => TogglePlayerMute(__instance)));
                //else
                //    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Mute"), new Action(() => TogglePlayerMute(__instance)));

                // Check for Host and Other Players
                if (areYouHosting)
                {
                    // Create Kick Button
                    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Kick"), new Action(__instance.playerLobbyUI.KickPlayer));

                    // Create Blacklist Button
                    __instance.InstantiateButton(LocalizationManager.instance.GetLocalizedValue("Blacklist"), new Action(__instance.playerLobbyUI.BlacklistPlayer));
                }
            }

            // Fix Panel Size
            __instance.backgroundImage.sizeDelta = new Vector2(
                __instance.backgroundImage.sizeDelta.x,
                86.2f + (__instance.childCount - 1) * 34.7f);

            // Prevent Original
            return false;
        }

        /*
        private static void TogglePlayerMute(PlayerAlternatives alternatives)
        {
            alternatives.playerLobbyUI.lobbyPlayer.SetMuted(!alternatives.playerLobbyUI.lobbyPlayer.isMuted);
            alternatives.UpdateTab();
        }
        */
    }
}