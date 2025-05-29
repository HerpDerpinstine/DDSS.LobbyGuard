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
            List<ILobbyModule> validModules = new();
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

                // Load Preferences
                Type configType = module.ConfigType;
                if (configType != null)
                    module.Config = (ConfigCategory)Activator.CreateInstance(configType);

                // Add Module to Cache
                validModules.Add(module);
            }

            // Sort Modules by Priority
            validModules = validModules.OrderBy(item => item.Priority).ToList();

            foreach (var module in validModules)
            {
                string moduleName = module.Name;

#if DEBUG
                MelonMain._logger.Msg($"Loading {moduleName}...");
#endif

                // Run OnLoad
                if (!module.OnLoad())
                {
#if DEBUG
                    MelonMain._logger.Error($"{moduleName} OnLoad returned false");
#endif
                    module.OnQuit();
                    if (module.Config != null)
                        ConfigCategory._allCategories.Remove(module.Config.CategoryID);
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
                        ConfigCategory._allCategories.Remove(module.Config.CategoryID);
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

        internal static void SceneInit(int buildIndex, string sceneName)
        {
            foreach (var module in _modules)
                module.OnSceneInit(buildIndex, sceneName);
        }
    }
}
