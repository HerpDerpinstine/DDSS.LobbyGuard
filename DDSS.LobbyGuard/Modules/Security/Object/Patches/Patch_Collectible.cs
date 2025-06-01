using HarmonyLib;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Collectible
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Collectible), nameof(Collectible.Start))]
        private static void Start_Postfix(Collectible __instance)
        {
            if (__instance.despawnAfterIdle)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleCollectibles.Value;
        }
    }
}