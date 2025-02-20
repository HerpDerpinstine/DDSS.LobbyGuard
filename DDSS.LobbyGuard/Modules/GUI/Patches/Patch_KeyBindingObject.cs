using DDSS_LobbyGuard.Modules.GUI.Internal;
using HarmonyLib;
using Il2CppUI.Tabs.SettingsTab;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.GUI.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_KeyBindingObject
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.SetKeyBind))]
        private static bool SetKeyBind_Prefix(KeyBindingObject __instance)
        {
            // Check for KeyCode Binds
            if (!__instance.actionName.StartsWith(ModSettingsOptionBuilder._keyCodePrefix))
                return true;

            // Update Value Text
            __instance.keyBindText.text = __instance.actionName.Substring(ModSettingsOptionBuilder._keyCodePrefixLen,
                __instance.actionName.Length - ModSettingsOptionBuilder._keyCodePrefixLen);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.RefreshKeyBindText))]
        private static bool RefreshKeyBindText_Prefix(KeyBindingObject __instance)
        {
            // Check for KeyCode Binds
            if (!__instance.actionName.StartsWith(ModSettingsOptionBuilder._keyCodePrefix))
                return true;

            // Update Value Text
            __instance.keyBindText.text = __instance.actionName.Substring(ModSettingsOptionBuilder._keyCodePrefixLen,
                __instance.actionName.Length - ModSettingsOptionBuilder._keyCodePrefixLen);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(KeyBindingObject), nameof(KeyBindingObject.UpdateKeyBind))]
        private static bool UpdateKeyBind_Prefix(KeyBindingObject __instance)
        {
            // Check for KeyCode Binds
            if (!__instance.actionName.StartsWith(ModSettingsOptionBuilder._keyCodePrefix))
                return true;

            // Wait for User Input
            ModSettingsManager.StartRebind(
                // Found Key
                (newCode) =>
                {
                    if (newCode != KeyCode.Escape)
                        __instance.actionName = $"{ModSettingsOptionBuilder._keyCodePrefix}{Enum.GetName(newCode)}";
                    __instance.RefreshKeyBindText();
                },
                // Rebind Cancelled (manually or timeout)
                () => __instance.RefreshKeyBindText(),
                // Every Check Tick
                (timeLeft) => __instance.keyBindText.text = $"[ {timeLeft} ]");

            // Prevent Original
            return false;
        }
    }
}