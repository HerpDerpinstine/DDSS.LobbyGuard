﻿using HarmonyLib;
using Il2CppSteamworks;

namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateLobbies.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_SteamMatchmaking
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.RequestLobbyList))]
        private static void RequestLobbyList_Prefix()
        {
            Patch_LobbyItem._lobbyList.Clear();
        }
    }
}
