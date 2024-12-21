using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Keys;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_KeyHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyHolder), nameof(KeyHolder.Start))]
        private static bool Start_Prefix(KeyHolder __instance)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;
            if (!ConfigHandler.Gameplay.SpawnManagerKeys.Value)
                return false;

            // Key Security
            CollectibleSecurity.SpawnKey(__instance);

            // Prevent Original
            return false;
        }
    }
}
