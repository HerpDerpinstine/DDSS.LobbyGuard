using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Door;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerDetectionVolume
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerDetectionVolume), nameof(PlayerDetectionVolume.OnTriggerEnter))]
        private static bool OnTriggerEnter_Prefix(PlayerDetectionVolume __instance, Collider __0)
        {
            DoorSecurity.FixColliderSize(__instance);

            if (!NetworkServer.activeHost)
                return true;

            if (!PlayerTriggerSecurity.IsColliderValid(__0))
                return false;

            PlayerTriggerSecurity.OnEnter(__instance, __0);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerDetectionVolume), nameof(PlayerDetectionVolume.OnTriggerExit))]
        private static bool OnTriggerExit_Prefix(PlayerDetectionVolume __instance, Collider __0)
        {
            if (!NetworkServer.activeHost)
                return true;

            // Prevent Original
            return false;
        }
    }
}
