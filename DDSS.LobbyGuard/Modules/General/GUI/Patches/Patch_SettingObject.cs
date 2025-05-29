using DDSS_LobbyGuard.Modules.General.GUI;
using DDSS_LobbyGuard.Modules.General.GUI.Internal;
using HarmonyLib;
using Il2CppUI.Tabs.SettingsTab;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.General.GUI.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_SettingObject
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.DisplaySetting))]
        private static bool DisplaySetting_Prefix(SettingObject __instance)
        {
            // Check for Custom Setting Types
            if (__instance.setting.axisName != "MODDED"
                || __instance.setting.alternatives == null)
                return true;

            // Enable Multi-Choice
            __instance.multiChoice.SetActive(true);
            __instance.slider.gameObject.SetActive(false);
            __instance.keyBind.gameObject.SetActive(false);
            __instance.UpdateMultiChoice();
            __instance.settingName.text = __instance.setting.Key;
            __instance.additionalInfoText.text = __instance.setting.additionalInfo;
            __instance.value = __instance.setting.Value;
            __instance.prevValue = __instance.value;

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.ChangeMultiChoice))]
        private static bool ChangeMultiChoice_Prefix(SettingObject __instance, int __0)
        {
            // Check for Custom Setting Types
            if (__instance.setting.axisName != "MODDED"
                || __instance.setting.alternatives == null
                || !ModSettingsFactory._settingCache.TryGetValue(__instance, out MelonPreferences_Entry entry))
                return true;

            // Increment the Setting
            __instance.setting.Value =
                ModSettingsManager.IncrementCustomSettingValue(
                    entry.GetReflectedType(),
                    __0,
                    __instance.setting.Value,
                    __instance.setting.alternatives[1].key,
                    __instance.setting.alternatives[2].key);
            __instance.UpdateMultiChoice();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.UpdateMultiChoice))]
        private static bool UpdateMultiChoice_Prefix(SettingObject __instance)
        {
            // Check for Custom Setting Types
            if (__instance.setting.axisName != "MODDED"
                || __instance.setting.alternatives == null)
                return true;

            // Update Multi-Choice
            __instance.multiChoiceText.text = __instance.setting.Value.ToString();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SettingObject), nameof(SettingObject.ApplySetting))]
        private static bool ApplySetting_Prefix(SettingObject __instance, ref bool __0)
        {
            // Apply Value
            if (__instance.setting.Value != __instance.value)
                __instance.setting.Value = __instance.value;

            // Preset Verification
            if (__0
                && __instance.setting.presetName != string.Empty
                && !__instance.blockPresets)
                __instance.settingsManager.currentPreset.VerifyPreset(
                    __instance.setting,
                    __instance.settingsManager);

            // Prevent Original
            return false;
        }
    }
}