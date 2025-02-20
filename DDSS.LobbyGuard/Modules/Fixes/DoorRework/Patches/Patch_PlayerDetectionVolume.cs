using HarmonyLib;
using Il2CppProps.Door;

namespace DDSS_LobbyGuard.Modules.Fixes.DoorRework.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PlayerDetectionVolume
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerDetectionVolume), nameof(PlayerDetectionVolume.OnTriggerEnter))]
        private static void OnTriggerEnter_Prefix(PlayerDetectionVolume __instance)
            => ModuleMain.FixColliderSize(__instance);
    }
}
