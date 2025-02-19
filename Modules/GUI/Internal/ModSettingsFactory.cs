using DDSS_LobbyGuard.Config;
using Il2Cpp;
using Il2CppUI.Tabs.SettingsTab;
using MelonLoader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.GUI.Internal
{
    internal static class ModSettingsFactory
    {
        internal static Dictionary<SettingObject, MelonPreferences_Entry> _settingCache = new();
        internal static Dictionary<eConfigType, CategoryButton> _categoryCache = new();

        internal static void Reset()
        {
            ModSettingsManager.CancelRebind();

            // Reset Settings to Defaults
            foreach (KeyValuePair<SettingObject, MelonPreferences_Entry> pair in _settingCache)
            {
                SettingObject setting = pair.Key;
                MelonPreferences_Entry melonEntry = pair.Value;

                setting.value = setting.prevValue;
                melonEntry.ResetToDefault();
            }

            // Save Categories
            foreach (var cat in ConfigCategory._allCategories.Values)
                cat.Save();

            // Reset Objects
            ModSettingsManager._tab.ShowSettings();

            // Log Changes
            MelonMain._logger.Msg("Settings have been Reset!");
        }

        internal static void Apply()
        {
            ModSettingsManager.CancelRebind();

            // Apply Setting Changes
            foreach (KeyValuePair<SettingObject, MelonPreferences_Entry> pair in _settingCache)
            {
                SettingObject setting = pair.Key;
                MelonPreferences_Entry melonEntry = pair.Value;

                Type melonEntryType = melonEntry.GetReflectedType();
                float currentValue = setting.setting.Value;

                if (melonEntryType == typeof(KeyCode))
                {
                    string valueStr = setting.keyBind.actionName.Substring(ModSettingsOptionBuilder._keyCodePrefixLen,
                        setting.keyBind.actionName.Length - ModSettingsOptionBuilder._keyCodePrefixLen);
                    KeyCode[] enumValues = Enum.GetValues<KeyCode>();
                    foreach (KeyCode obj in enumValues)
                    {
                        if (Enum.GetName(obj) != valueStr)
                            continue;
                        melonEntry.BoxedValue = melonEntry.BoxedEditedValue = obj;
                        break;
                    }
                }
                else if (melonEntryType.IsEnum)
                {
                    int valueIndex = (int)currentValue;
                    Array enumValues = Enum.GetValues(melonEntryType);
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = enumValues.GetValue(valueIndex);
                }
                else if (melonEntryType == typeof(bool))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = currentValue > 0;
                else if (melonEntryType == typeof(int))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = (int)currentValue;
                else if (melonEntryType == typeof(uint))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = (uint)currentValue;
                else if (melonEntryType == typeof(short))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = (short)currentValue;
                else if (melonEntryType == typeof(ushort))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = (ushort)currentValue;
                else if (melonEntryType == typeof(float))
                    melonEntry.BoxedValue = melonEntry.BoxedEditedValue = currentValue;

                setting.value = currentValue;
                setting.prevValue = currentValue;
            }

            // Save Categories
            foreach (var cat in ConfigCategory._allCategories.Values)
                cat.Save();

            // Log Changes
            MelonMain._logger.Msg("Settings have been Saved!");
        }

        internal static void GenerateCategories()
        {
            // Clear Old Listings
            _categoryCache.Clear();
            int childCount = ModSettingsManager._tab.categoryGrid.childCount;
            if (childCount > 0)
                for (int i = 0; i < childCount; i++)
                    UnityEngine.Object.Destroy(ModSettingsManager._tab.categoryGrid.GetChild(i).gameObject);

            // Add New Listings
            int index = 0;
            foreach (var cat in Enum.GetValues<eConfigType>())
            {
                // Create Category Button
                string catName = Enum.GetName(cat);
                CategoryButton category = ModSettingsCategoryBuilder.CreateButton(catName);
                _categoryCache[cat] = category;
                category.categoryIndex = index;
                category.SetSelected(ModSettingsManager._tab.currentCategoryIndex);
                category.button.enabled = (ModSettingsManager._tab.currentCategoryIndex != index);
                category.button.SetHighlighted((ModSettingsManager._tab.currentCategoryIndex == index));

                // Fix Button
                category.button.OnClick = new();
                category.button.OnClick.AddListener(new Action(() => OnCategorySelected(category)));

                // Increase Index
                index++;
            }
        }

        internal static void OnCategorySelected(CategoryButton button)
        {
            ModSettingsManager._tab.SelectCategory(button.categoryIndex);
            button.SetSelected(button.categoryIndex);
            button.button.enabled = false;
            button.button.SetHighlighted(true);

            foreach (var cat in _categoryCache.Values)
            {
                cat.SetSelected(button.categoryIndex);
                cat.button.enabled = true;
                cat.button.SetHighlighted(false);
            }
        }

        internal static void Generate()
        {
            // Clear Old Listings
            _settingCache.Clear();
            int childCount = ModSettingsManager._tab.settingsParent.childCount;
            if (childCount > 0)
                for (int i = 0; i < childCount; i++)
                    UnityEngine.Object.Destroy(ModSettingsManager._tab.settingsParent.GetChild(i).gameObject);

            // Add New Listings
            foreach (var cat in ConfigCategory._allCategories.Values)
            {
                if (ModSettingsManager._tab.currentCategoryIndex != (int)cat.ConfigType)
                    continue;

                MelonPreferences_Category melonCat = cat.Category;

                // Create Category Object
                ModSettingsCategoryBuilder.Create(melonCat.DisplayName);

                // Iterate through Entries
                foreach (MelonPreferences_Entry melonEntry in melonCat.Entries)
                {
                    // Skip Hidden Entries
                    if (melonEntry.IsHidden)
                        continue;

                    // Create Entry Objects
                    Type melonEntryType = melonEntry.GetReflectedType();
                    if (melonEntryType == typeof(KeyCode))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateKeybind(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (KeyCode)melonEntry.BoxedValue),
                            melonEntry);
                    else if (melonEntryType.IsEnum)
                        _settingCache.Add(ModSettingsOptionBuilder.CreateEnum(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            melonEntry.GetValueAsString(),
                            melonEntryType),
                            melonEntry);
                    else if (melonEntryType == typeof(bool))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateToggle(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (bool)melonEntry.BoxedValue),
                            melonEntry);
                    else if (melonEntryType == typeof(int))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (int)melonEntry.BoxedValue,
                            int.MinValue,
                            int.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(uint))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (uint)melonEntry.BoxedValue,
                            uint.MinValue,
                            uint.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(float))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (float)melonEntry.BoxedValue,
                            float.MinValue,
                            float.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(short))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (short)melonEntry.BoxedValue,
                            short.MinValue,
                            short.MaxValue),
                            melonEntry);
                    else if (melonEntryType == typeof(ushort))
                        _settingCache.Add(ModSettingsOptionBuilder.CreateNumber(
                            melonEntry.DisplayName,
                            string.Empty, // melonEntry.Description,
                            (ushort)melonEntry.BoxedValue,
                            ushort.MinValue,
                            ushort.MaxValue),
                            melonEntry);
                }
            }

            // Scroll To Top
            ModSettingsManager._tab.StartCoroutine(ModSettingsManager._tab.ScrollToTopNextFrame());
        }
    }
}
