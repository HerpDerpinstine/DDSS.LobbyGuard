using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppProps.Keys;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_KeyHolder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyHolder), nameof(KeyHolder.Start))]
        private static bool Start_Prefix(KeyHolder __instance)
        {
            // Key Security
            KeySecurity.SpawnKey(__instance);

            // Prevent Original
            return false;
        }
    }
}
