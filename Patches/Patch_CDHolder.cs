using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Stereo;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_CDHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CDHolder), nameof(CDHolder.Start))]
        private static bool Start_Prefix(CDHolder __instance)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;
            if (!ConfigHandler.Gameplay.SpawnStereoCDs.Value)
                return false;

            // Security
            CollectibleSecurity.SpawnCDStart(__instance);

            // Prevent Original
            return false;
        }
    }
}
