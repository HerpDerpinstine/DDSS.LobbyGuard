using Il2Cpp;
using Il2CppLocalization;
using Il2CppSystem.Collections;
using Il2CppTMPro;
using Il2CppUI.Tabs.SettingsTab;
using Il2CppUMUI;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    // It's name is Bob
    internal abstract class ModSettingsBuilder
    {
        #region Internal Members

        internal UiTab _tab;
        internal GameObject _tabObj;
        internal SettingsTab _tabCasted;
        internal static GameObject _tabStatic;

        internal UiTab _panelTab;
        internal GameObject _panelTabObj;
        internal static GameObject _panelTabStatic;

        internal GameObject _categoryPrefab;
        internal GameObject _categoryButtonPrefab;
        internal Transform _categoryGrid;

        internal GameObject _settingPrefab;
        internal Transform _settingParent;
        internal RectTransform _settingParentRect;

        internal int currentCategoryIndex
        {
            get => _tabCasted.currentCategoryIndex;
            set => _tabCasted.currentCategoryIndex = value;
        }

        #endregion

        #region Internal Methods

        internal ModSettingsBuilder()
        {
            CreatePanel();
            CreateTab();

            if ((_panelTabStatic == null)
                || _panelTabStatic.WasCollected)
            {
                _panelTabStatic = GameObject.Instantiate(_panelTabObj);
                _panelTabStatic.SetActive(false);
                GameObject.DontDestroyOnLoad(_panelTabStatic);
            }

            if ((_tabStatic == null)
                || _tabStatic.WasCollected)
            {
                _tabStatic = GameObject.Instantiate(_tabObj);
                _tabStatic.SetActive(false);
                GameObject.DontDestroyOnLoad(_tabStatic);
            }
        }

        internal abstract void CreateTab();
        internal abstract void CreatePanel();

        internal void ShowSettings()
            => _tabCasted.ShowSettings();

        internal void SelectCategory(int categoryIndex)
            => _tabCasted.SelectCategory(categoryIndex);

        internal IEnumerator ScrollToTopNextFrame()
            => _tabCasted.ScrollToTopNextFrame();

        #endregion

        #region Category

        internal GameObject CreateCategory(string name)
        {
            if ((_categoryPrefab == null)
                || _categoryPrefab.WasCollected
                || (_settingParent == null)
                || _settingParent.WasCollected)
                return null;

            GameObject catObj = UnityEngine.Object.Instantiate(_categoryPrefab, _settingParent);
            TextMeshProUGUI catTmp = catObj.GetComponentInChildren<TextMeshProUGUI>();
            catTmp.text = name;
            catTmp.autoSizeTextContainer = true;
            return catObj;
        }
        internal CategoryButton CreateCategoryButton(string name)
        {
            GameObject catObj = UnityEngine.Object.Instantiate(_categoryButtonPrefab, _categoryGrid);
            TextMeshProUGUI catTmp = catObj.GetComponentInChildren<TextMeshProUGUI>();
            catTmp.text = name;
            return catObj.GetComponent<CategoryButton>();
        }

        #endregion

        #region Setting

        private SettingObject CreateSettingObject(Setting setting)
        {
            GameObject obj = UnityEngine.Object.Instantiate(_settingPrefab, _settingParent);
            SettingObject settingObj = obj.GetComponent<SettingObject>();
            settingObj.SetSetting(setting, GameSettingsManager.instance, true, true);

            LocalizedText localizedText = obj.GetComponent<LocalizedText>();
            if (localizedText != null)
                UnityEngine.Object.Destroy(localizedText);

            return settingObj;
        }

        private Setting CreateSetting(string name,
            string description,
            SettingType type = SettingType.MultiChoice)
        {
            Setting setting = new();
            setting.devOnly = false;
            setting.label = name;
            setting.Key = name;
            setting.presetName = string.Empty;
            setting.type = type;
            setting.additionalInfo = description;
            return setting;
        }

        internal SettingObject CreateSettingKeybind(string name,
            string description,
            KeyCode value)
        {
            Setting setting = CreateSetting(name, description, SettingType.KeyBind);
            setting.axisName = $"{ModSettingsManager._keyCodePrefix}{Enum.GetName(typeof(KeyCode), value)}";
            SettingObject obj = CreateSettingObject(setting);
            obj.keyBind.RefreshKeyBindText();
            return obj;
        }

        internal SettingObject CreateSettingToggle(string name,
            string description,
            bool value)
        {
            Setting setting = CreateSetting(name, description);
            setting.alternatives = new();
            setting.alternatives.Add(new("OFF", "OFF"));
            setting.alternatives.Add(new("ON", "ON"));
            setting.Value = value ? 1f : 0f;
            return CreateSettingObject(setting);
        }

        internal SettingObject CreateSettingNumber<T>(
            string name,
            string description,
            T value,
            T minValue,
            T maxValue)
            where T : IConvertible
        {
            Setting setting = CreateSetting(name, description);
            setting.axisName = "MODDED";
            setting.alternatives = new();
            setting.alternatives.Add(new(typeof(float).FullName, typeof(float).FullName));
            setting.alternatives.Add(new(minValue.ToString(), minValue.ToString()));
            setting.alternatives.Add(new(maxValue.ToString(), maxValue.ToString()));

            if (typeof(T) == typeof(float))
                setting.Value = (float)Math.Round(Convert.ToDouble(value), 1, MidpointRounding.AwayFromZero);
            else
                setting.Value = Convert.ToSingle(value);

            return CreateSettingObject(setting);
        }

        internal SettingObject CreateSettingEnum(string name,
            string description,
            string value,
            Type enumType)
        {
            Setting setting = CreateSetting(name, description);
            setting.alternatives = new();

            string[] valueNames = Enum.GetNames(enumType);
            foreach (string valueName in valueNames)
                setting.alternatives.Add(new(valueName, valueName));

            int valueIndex = 0;
            Array allValues = Enum.GetValues(enumType);
            for (int i = 0; i < allValues.Length; i++)
            {
                if (allValues.GetValue(i).ToString() != value)
                    continue;
                valueIndex = i;
                break;
            }
            setting.Value = valueIndex;

            return CreateSettingObject(setting);
        }

        #endregion
    }
}
