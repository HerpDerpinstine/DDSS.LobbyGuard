using DDSS_LobbyGuard.Config;
using Il2Cpp;
using Il2CppLocalization;
using Il2CppTMPro;
using Il2CppUI.Tabs.SettingsTab;
using Il2CppUMUI.UiElements;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    internal static class ModSettingsFactory
    {
        internal static Dictionary<SettingObject, MelonPreferences_Entry> _settingCache = new();
        internal static Dictionary<eModuleType, CategoryButton> _categoryCache = new();

        internal static void Reset()
        {
            ModSettingsManager.CancelRebind();

            // Reset Settings to Defaults
            foreach (var cat in ConfigCategory._allCategories.Values)
            {
                // Iterate through Entries
                MelonPreferences_Category melonCat = cat.Category;
                foreach (MelonPreferences_Entry melonEntry in melonCat.Entries)
                    melonEntry.ResetToDefault();
            }

            // Save Categories
            foreach (var cat in ConfigCategory._allCategories.Values)
                cat.Save();

            // Reset Objects
            ModSettingsManager._builder.ShowSettings();

            // Log Changes
            MelonMain._logger.Msg("Settings have been Reset!");
        }

        internal static void Apply()
        {
            ModSettingsManager.CancelRebind();

            CacheSettingChanges();

            // Apply Setting Changes
            foreach (var cat in ConfigCategory._allCategories.Values)
            {
                // Iterate through Entries
                MelonPreferences_Category melonCat = cat.Category;
                foreach (MelonPreferences_Entry melonEntry in melonCat.Entries)
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue;
            }

            // Save Categories
            foreach (var cat in ConfigCategory._allCategories.Values)
                cat.Save();

            // Log Changes
            MelonMain._logger.Msg("Settings have been Saved!");
        }

        internal static void OnClose()
        {
            foreach (var cat in ConfigCategory._allCategories.Values)
                foreach (var entry in cat.Category.Entries)
                    entry.BoxedEditedValue = entry.BoxedValue;
        }

        internal static void GenerateCategories()
        {
            // Clear Old Listings
            _categoryCache.Clear();
            int childCount = ModSettingsManager._builder._categoryGrid.childCount;
            if (childCount > 0)
                for (int i = 0; i < childCount; i++)
                    UnityEngine.Object.Destroy(ModSettingsManager._builder._categoryGrid.GetChild(i).gameObject);

            // Add New Listings
            int index = 0;
            foreach (var cat in Enum.GetValues<eModuleType>())
            {
                // Create Category Button
                string catName = Enum.GetName(cat);
                CategoryButton category = ModSettingsManager._builder.CreateCategoryButton(catName);
                _categoryCache[cat] = category;
                category.categoryIndex = index;
                category.SetSelected(ModSettingsManager._builder.currentCategoryIndex);
                category.button.enabled = ModSettingsManager._builder.currentCategoryIndex != index;
                category.button.SetHighlighted(ModSettingsManager._builder.currentCategoryIndex == index);

                // Fix Button
                category.button.OnClick = new();
                category.button.OnClick.AddListener(new Action(() => OnCategorySelected(category)));

                // Increase Index
                index++;
            }
        }

        private static void CacheSettingChanges()
        {
            // Cache Setting Changes
            foreach (KeyValuePair<SettingObject, MelonPreferences_Entry> pair in _settingCache)
            {
                SettingObject setting = pair.Key;
                MelonPreferences_Entry melonEntry = pair.Value;

                Type melonEntryType = melonEntry.GetReflectedType();
                float currentValue = setting.setting.Value;

                if (melonEntryType == typeof(KeyCode))
                {
                    string valueStr = setting.keyBind.actionName.Substring(ModSettingsManager._keyCodePrefixLen,
                        setting.keyBind.actionName.Length - ModSettingsManager._keyCodePrefixLen);
                    KeyCode[] enumValues = Enum.GetValues<KeyCode>();
                    foreach (KeyCode obj in enumValues)
                    {
                        if (Enum.GetName(obj) != valueStr)
                            continue;
                        melonEntry.BoxedEditedValue = obj;
                        break;
                    }
                }
                else if (melonEntryType.IsEnum)
                {
                    int valueIndex = (int)currentValue;
                    Array enumValues = Enum.GetValues(melonEntryType);
                    melonEntry.BoxedEditedValue = enumValues.GetValue(valueIndex);
                }
                else if (melonEntryType == typeof(bool))
                    melonEntry.BoxedEditedValue = currentValue > 0;
                else if (melonEntryType == typeof(int))
                    melonEntry.BoxedEditedValue = (int)currentValue;
                else if (melonEntryType == typeof(uint))
                    melonEntry.BoxedEditedValue = (uint)currentValue;
                else if (melonEntryType == typeof(short))
                    melonEntry.BoxedEditedValue = (short)currentValue;
                else if (melonEntryType == typeof(ushort))
                    melonEntry.BoxedEditedValue = (ushort)currentValue;
                else if (melonEntryType == typeof(float))
                    melonEntry.BoxedEditedValue = currentValue;

                setting.value = currentValue;
                setting.prevValue = currentValue;
            }
        }

        internal static void OnCategorySelected(CategoryButton button)
        {
            // Cache Setting Changes
            CacheSettingChanges();

            // Select New Category
            ModSettingsManager._builder.SelectCategory(button.categoryIndex);
            button.SetSelected(button.categoryIndex);
            button.button.enabled = false;
            button.button.SetHighlighted(true);

            // Unselect Other Categories
            foreach (var cat in _categoryCache.Values)
            {
                cat.SetSelected(button.categoryIndex);
                cat.button.enabled = true;
                cat.button.SetHighlighted(false);
            }

            // Scroll To Top
            ModSettingsManager._builder._tab.StartCoroutine(ModSettingsManager._builder.ScrollToTopNextFrame());
        }

        internal static void SetupButton(Transform trans, Vector3 localPos, string text, Action onClick)
        {
            // Move Object
            trans.localPosition = localPos;

            // Rename Object
            trans.name = text;

            // Set Text
            TextMeshProUGUI creditsText = trans.GetComponentInChildren<TextMeshProUGUI>();
            if (creditsText != null)
                creditsText.text = text;

            // Remove Localization
            LocalizedText localized = trans.GetComponentInChildren<LocalizedText>();
            if (localized != null)
                UnityEngine.Object.Destroy(localized);

            // Apply new Button Callback
            UMUIButton cloneSettingsButton = trans.GetComponentInChildren<UMUIButton>();
            if (cloneSettingsButton != null)
            {
                // Create Event to Open Custom Tab
                cloneSettingsButton.OnClick = new();
                cloneSettingsButton.OnClick.AddListener(onClick);
            }
        }

        internal static void Generate()
        {
            // Clear Old Listings
            _settingCache.Clear();
            int childCount = ModSettingsManager._builder._settingParent.childCount;
            if (childCount > 0)
                for (int i = 0; i < childCount; i++)
                    UnityEngine.Object.Destroy(ModSettingsManager._builder._settingParent.GetChild(i).gameObject);

            // Add New Listings
            foreach (var cat in ConfigCategory._allCategories.Values)
            {
                if (ModSettingsManager._builder.currentCategoryIndex != (int)cat.ConfigType)
                    continue;

                MelonPreferences_Category melonCat = cat.Category;

                // Create Category Object
                ModSettingsManager._builder.CreateCategory(melonCat.DisplayName);

                // Iterate through Entries
                foreach (MelonPreferences_Entry melonEntry in melonCat.Entries)
                {
                    // Skip Hidden Entries
                    if (melonEntry.IsHidden)
                        continue;

                    // Create Entry Objects
                    Type melonEntryType = melonEntry.GetReflectedType();
                    if (melonEntryType == typeof(KeyCode))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingKeybind(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (KeyCode)melonEntry.BoxedEditedValue),
                            melonEntry);
                    else if (melonEntryType.IsEnum)
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingEnum(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            melonEntry.GetEditedValueAsString(),
                            melonEntryType),
                            melonEntry);
                    else if (melonEntryType == typeof(bool))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingToggle(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (bool)melonEntry.BoxedEditedValue),
                            melonEntry);
                    else if (melonEntryType == typeof(int))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (int)melonEntry.BoxedEditedValue,
                            int.MinValue,
                            int.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(uint))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (uint)melonEntry.BoxedEditedValue,
                            uint.MinValue,
                            uint.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(float))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (float)melonEntry.BoxedEditedValue,
                            float.MinValue,
                            float.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(short))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (short)melonEntry.BoxedEditedValue,
                            short.MinValue,
                            short.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(ushort))
                        _settingCache.Add(ModSettingsManager._builder.CreateSettingNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (ushort)melonEntry.BoxedEditedValue,
                            ushort.MinValue,
                            ushort.MaxValue),
                            melonEntry);
                }
            }

            // Scroll To Top
            ModSettingsManager._builder._tab.StartCoroutine(ModSettingsManager._builder.ScrollToTopNextFrame());
        }
    }
}
