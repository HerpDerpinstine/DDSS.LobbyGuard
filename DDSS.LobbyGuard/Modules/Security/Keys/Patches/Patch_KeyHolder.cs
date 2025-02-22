using DDSS_LobbyGuard.SecurityExtension;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Keys;

namespace DDSS_LobbyGuard.Modules.Security.Keys.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
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
            if (!ModuleConfig.Instance.SpawnManagerKeys.Value)
                return false;

            // Key Security
            CollectibleSecurity.SpawnKey(__instance);

            // Prevent Original
            return false;
        }

    }
}
