using HarmonyLib;
using Il2CppProps.Door;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Fixes.DoorRework.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PlayerDetectionVolume
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerDetectionVolume), nameof(PlayerDetectionVolume.OnTriggerEnter))]
        private static bool OnTriggerEnter_Prefix(PlayerDetectionVolume __instance, Collider __0)
        {
            ModuleMain.FixColliderSize(__instance);
            return __0 != null;
        }
    }
}
