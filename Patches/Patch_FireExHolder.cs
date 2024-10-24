﻿using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FireExHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FireExHolder), nameof(FireExHolder.Start))]
        private static bool Start_Prefix(FireExHolder __instance)
        {
            // Key Security
            FireExSecurity.SpawnFireEx(__instance);

            // Prevent Original
            return false;
        }
    }
}
