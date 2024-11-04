using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
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
            if ((LobbyManager.instance == null)
                || LobbyManager.instance.WasCollected
                || !LobbyManager.instance._localPlayer.isHost)
                return true;

            // Key Security
            KeySecurity.SpawnKey(__instance);

            // Prevent Original
            return false;
        }
    }
}
