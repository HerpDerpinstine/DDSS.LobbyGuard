using DDSS_LobbyGuard.SecurityExtension;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Collectible.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_FireExHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FireExHolder), nameof(FireExHolder.Start))]
        private static bool Start_Prefix(FireExHolder __instance)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;

            // Fire Extinguisher Security
            CollectibleSecurity.SpawnFireEx(__instance);

            // Prevent Original
            return false;
        }
    }
}