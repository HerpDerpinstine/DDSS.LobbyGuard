using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Shelf;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ShelfController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShelfController), nameof(ShelfController.Start))]
        private static bool Start_Prefix(ShelfController __instance)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;

            // Spawn Binders
            CollectibleSecurity.SpawnStorageBoxStart(__instance);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShelfController), nameof(ShelfController.SpawnBox))]
        private static bool SpawnBox_Prefix(ShelfController __instance, int __0)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;

            // Spawn Binders
            CollectibleSecurity.SpawnStorageBox(__instance, __0);

            // Prevent Original
            return false;
        }
    }
}
