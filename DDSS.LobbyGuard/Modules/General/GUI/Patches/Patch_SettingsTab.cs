using DDSS_LobbyGuard.Modules.General.GUI;
using DDSS_LobbyGuard.Modules.General.GUI.Internal;
using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.General.GUI.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_SettingsTab
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ShowSettings))]
        private static bool ShowSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Generate Entries
            ModSettingsFactory.Generate();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ShowCategories))]
        private static bool ShowCategories_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Generate Categories
            ModSettingsFactory.GenerateCategories();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ApplyAllSettings))]
        private static bool ApplyAllSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Apply Changes
            ModSettingsFactory.Apply();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingsTab), nameof(SettingsTab.ResetAllSettings))]
        private static bool ResetAllSettings_Prefix(SettingsTab __instance)
        {
            // Check for Mod Settings Tab
            if (__instance != ModSettingsManager._tab)
                return true;

            // Reset to Default
            ModSettingsFactory.Reset();

            // Prevent Original
            return false;
        }
    }
}