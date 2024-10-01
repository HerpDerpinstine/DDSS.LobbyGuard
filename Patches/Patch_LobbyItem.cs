using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_LobbyItem
    {
        internal static List<CSteamID> _lobbyList = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyItem), nameof(LobbyItem.SetLobby))]
        private static bool SetLobby_Prefix(LobbyItem __instance, CSteamID __0, ref string __1)
        {
            // Check if Host already has a Lobby Shown
            if (_lobbyList.Contains(__0))
            {
                // Remove This Listing
                GameObject.Destroy(__instance.gameObject);

                // Prevent Original
                return false;
            }

            // Add Lobby to Cache
            _lobbyList.Add(__0);

            // Run Original
            return true;
        }
    }
}