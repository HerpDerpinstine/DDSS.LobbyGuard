﻿using HarmonyLib;
using Il2Cpp;
using Il2CppSteamworks;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateLobbies.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyItem
    {
        internal static Dictionary<string, CSteamID> _lobbyList = new();

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyItem), nameof(LobbyItem.SetLobby))]
        private static bool SetLobby_Prefix(LobbyItem __instance, CSteamID __0, ref string __1)
        {
            // Get Lobby Host Address
            string address = SteamMatchmaking.GetLobbyData(__0, "HostAdress");
            if (string.IsNullOrEmpty(address)
                || string.IsNullOrWhiteSpace(address))
                return false;

            // Hide our own ghost lobby
            string userSteamId = SteamUser.GetSteamID().ToString();
            if (address == userSteamId)
                return false;

            // Check if Lobby is already in Cache
            if (_lobbyList.ContainsKey(address))
            {
                // Remove This Listing
                Object.Destroy(__instance.gameObject);

                // Prevent Original
                return false;
            }

            // Add Lobby to Cache
            _lobbyList[address] = __0;

            // Run Original
            return true;
        }
    }
}
