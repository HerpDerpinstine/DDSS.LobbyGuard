using DDSS_LobbyGuard.Config;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DDSS_LobbyGuard.Modules
{
    internal static class LobbyModuleManagerConfig
    {
        internal static Dictionary<string, ConfigCategory> _allCategories = new();
        internal static Dictionary<string, Dictionary<string, MelonPreferences_Entry<bool>>> _allToggles = new();

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

            foreach (var category in _allCategories.Values)
                category.Save();
        }

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
    }
}
