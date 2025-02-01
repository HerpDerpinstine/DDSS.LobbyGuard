using HarmonyLib;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkTransformBase
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkTransformBase), nameof(NetworkTransformBase.Apply))]
        private static void Apply_Prefix(NetworkTransformBase __instance)
        {
            if (!NetworkServer.activeHost)
                return;

            __instance.interpolatePosition = false;
            __instance.interpolateRotation = false;
            __instance.interpolateScale = false;
            __instance.onlySyncOnChange = false;
        }
    }
}
