using DDSS_LobbyGuard.Config;
using MelonLoader;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDSS_LobbyGuard.Modules
{
    internal static class LobbyModuleManagerConfig
    {
        private static ConfigCategory _internalCategory;
        private static MelonPreferences_Entry<string> _versionCheckEntry;

        internal static Dictionary<string, ConfigCategory> _allCategories = new();
        internal static Dictionary<string, Dictionary<string, MelonPreferences_Entry<bool>>> _allToggles = new();

        internal static bool IsModuleEnabled(ILobbyModule module)
            => IsModuleEnabled(module.ModuleType, module.Name);
        internal static bool IsModuleEnabled(eModuleType type, string moduleName)
        {
            string moduleTypeName = Enum.GetName(typeof(eModuleType), type);
            if (!_allToggles.TryGetValue(moduleTypeName, out Dictionary<string, MelonPreferences_Entry<bool>> category)
                || (category == null))
                return false;

            if (!category.TryGetValue(moduleName, out MelonPreferences_Entry<bool> pref)
                || (pref == null))
                return false;

            return pref.Value;
        }

        private static void CreateVersionCheck()
        {
            _internalCategory = new(MelonMain._userDataPath, "Modules", "INTERNAL", "INTERNAL", eModuleType.General);
            _versionCheckEntry = _internalCategory.CreatePref("LastUsedVersion", "LastUsedVersion", null, "0.0.0", true);
        }

        private static void HandleVersionCheck()
        {
            if (!SemVersion.TryParse(_versionCheckEntry.GetValueAsString(), out var lastUsedVersion)
                || !SemVersion.TryParse(BuildInfo.Version, out var currentVersion)
                || (currentVersion > lastUsedVersion))
                ResetPreferences();
        }

        private static void ResetPreferences()
        {
            _versionCheckEntry.EditedValue = _versionCheckEntry.Value = BuildInfo.Version;

            foreach (var togglePair in _allToggles.Values)
                foreach (var toggle in togglePair.Values)
                    toggle.ResetToDefault();

            _internalCategory.Save();
            foreach (var category in _allCategories.Values)
                category.Save();
        }

        internal static void CreatePreferences(SortedList<string, ILobbyModule> orderedList)
        {

            List<string> moduleTypeNames = Enum.GetNames(typeof(eModuleType)).ToList();
            moduleTypeNames.Sort();

            eModuleType[] moduleTypes = Enum.GetValues<eModuleType>();
            for (int i = 0; i < moduleTypeNames.Count; i++)
            {
                eModuleType moduleType = moduleTypes[i];
                string moduleTypeName = moduleTypeNames[i];
                if (!_allCategories.TryGetValue(moduleTypeName, out ConfigCategory category))
                    _allCategories[moduleTypeName] =
                        category = new(MelonMain._userDataPath, "Modules", moduleTypeName, moduleTypeName, moduleType);
            }

            foreach (ILobbyModule module in orderedList.Values)
            {
                string moduleName = module.Name;
                eModuleType moduleType = module.ModuleType;
                string moduleTypeName = Enum.GetName(typeof(eModuleType), moduleType);

                // Create Category
                if (!_allCategories.TryGetValue(moduleTypeName, out ConfigCategory category))
                    _allCategories[moduleTypeName] =
                        category = new(MelonMain._userDataPath, "Modules", moduleTypeName, moduleTypeName, moduleType);

                // Create Toggle Pair
                if (!_allToggles.TryGetValue(moduleTypeName, out Dictionary<string, MelonPreferences_Entry<bool>> togglePair)
                    || (togglePair == null))
                    _allToggles[moduleTypeName] = togglePair = new();

                // Create Toggle
                if (!togglePair.TryGetValue(moduleName, out MelonPreferences_Entry<bool> pref)
                    || (pref == null))
                    togglePair[moduleName] = pref = category.CreatePref(moduleName, moduleName, null, true);
            }

            CreateVersionCheck();

            foreach (var category in _allCategories.Values)
                category.Save();

            HandleVersionCheck();
        }
    }
}
