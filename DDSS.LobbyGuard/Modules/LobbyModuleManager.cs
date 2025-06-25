using DDSS_LobbyGuard.Config;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DDSS_LobbyGuard.Modules
{
    internal static class LobbyModuleManager
    {
        internal static List<ILobbyModule> _modules = new();

        internal static void Load()
        {
            Assembly asm = typeof(LobbyModuleManager).Assembly;
            SortedList<string, ILobbyModule> validModules = new();
            foreach (var type in asm.GetValidTypes())
            {
                // Find Module
                if (!type.IsSubclassOf(typeof(ILobbyModule)))
                    continue;

                // Create Module
                ILobbyModule module = (ILobbyModule)Activator.CreateInstance(type);
                if (module == null)
                    continue;

                if (module.IsDisabled)
                    continue;

                // Add Module to Cache
                string moduleTypeName = Enum.GetName(typeof(eModuleType), module.ModuleType);
                string moduleName = $"{moduleTypeName}.{module.Name}";
                validModules.Add(moduleName, module);
            }

            foreach (var module in validModules.Values)
            {
                // Load Preferences
                Type configType = module.ConfigType;
                if (configType != null)
                    module.Config = (ConfigCategory)Activator.CreateInstance(configType);
            }

            LobbyModuleManagerConfig.CreatePreferences(validModules);

            // Sort Modules by Priority
            var orderedValidModules = validModules.Values.OrderBy(item => item.Priority);
            foreach (var module in orderedValidModules)
            {
                string moduleTypeName = Enum.GetName(typeof(eModuleType), module.ModuleType);
                string moduleName = $"{moduleTypeName}.{module.Name}";

#if DEBUG
                MelonMain._logger.Msg($"Loading {moduleName}...");
#endif

                // Run OnLoad
                if (!LobbyModuleManagerConfig.IsModuleEnabled(module)
                    || module.IsDisabled
                    || !module.OnLoad())
                {
#if DEBUG
                    MelonMain._logger.Error($"{moduleName} OnLoad returned false");
#endif
                    module.OnQuit();
                    if (module.Config != null)
                        ConfigCategory._allCategories.Remove(module.Config.ID);
                    continue;
                }

                // Apply Module Patches
                module.HarmonyInstance = new(moduleName);
                if (!MelonMain.ApplyPatches(module, module.GetType()))
                {
#if DEBUG
                    MelonMain._logger.Error($"Failed to Apply Patches for {moduleName}");
#endif
                    module.HarmonyInstance.UnpatchSelf();
                    module.OnQuit();
                    if (module.Config != null)
                        ConfigCategory._allCategories.Remove(module.Config.ID);
                    continue;
                }

                // Add Module to Cache
                _modules.Add(module);
                MelonMain._logger.Msg($"Module Loaded: {moduleName}");
            }
        }

        internal static void Quit()
        {
            foreach (var module in _modules)
            {
                module.HarmonyInstance.UnpatchSelf();
                module.OnQuit();
            }
            _modules.Clear();
        }

        internal static void SceneLoad(int buildIndex, string sceneName)
        {
            foreach (var module in _modules)
                module.OnSceneLoad(buildIndex, sceneName);
        }

        internal static void SceneInit(int buildIndex, string sceneName)
        {
            foreach (var module in _modules)
                module.OnSceneInit(buildIndex, sceneName);
        }
    }
}
