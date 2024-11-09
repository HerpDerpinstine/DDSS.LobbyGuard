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
                return true;
            if (!NetworkServer.activeHost)
                return true;

            // Key Security
            KeySecurity.SpawnKey(__instance);

            // Prevent Original
            return false;
        }
    }
}
