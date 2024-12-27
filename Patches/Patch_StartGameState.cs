using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_StartGameState
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.Enter))]
        private static bool Enter_Prefix(StartGameState __instance)
        {
            // Prevent Original
            return false;
        }
    }
}
