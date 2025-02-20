using DDSS_LobbyGuard.Modules.GUI.Internal;
using HarmonyLib;
using Il2CppUMUI;

namespace DDSS_LobbyGuard.Modules.GUI.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_UIManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.OpenTab))]
        private static bool OpenTab_Prefix(string __0)
        {
            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Validate Name
            if (string.IsNullOrEmpty(__0)
                || string.IsNullOrWhiteSpace(__0))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.OpenFirstTab))]
        private static bool OpenFirstTab_Prefix()
        {
            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Reun Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIManager), nameof(UIManager.CloseTopTab))]
        private static bool CloseTopTab_Prefix()
        {
            // Check if Rebinding
            if (ModSettingsManager._rebindCoroutine != null)
            {
                ModSettingsManager.CancelRebind();
                return false;
            }

            // Reun Original
            return true;
        }
    }
}
