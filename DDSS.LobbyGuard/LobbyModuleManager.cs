using DDSS_LobbyGuard.Config;
using MelonLoader;
using MelonLoader.Pastel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace DDSS_LobbyGuard
{
    internal static class LobbyModuleManager
    {
        internal static string _modulePath { get; private set; }
        internal static List<ILobbyModule> _modules = new();

        internal static void Load()
        {
            // Setup Modules Folder
            _modulePath = Path.Combine(MelonMain._userDataPath, "Modules");
            if (!Directory.Exists(_modulePath))
                Directory.CreateDirectory(_modulePath);

            List<Assembly> validAssemblies = new();
            foreach (var filePath in Directory.GetFiles(_modulePath, "*.dll", SearchOption.AllDirectories))
            {
                Assembly asm = null;
                try
                {
                    asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
                }
                catch (Exception ex)
                {
                    continue;
                }
                if (asm == null)
                    continue;
                validAssemblies.Add(asm);
            }

            List<ILobbyModule> validModules = new();
            foreach (var asm in validAssemblies)
            {
                // Find Module
                ILobbyModule module = null;
                foreach (var type in asm.GetValidTypes())
                {
                    if (!type.IsSubclassOf(typeof(ILobbyModule)))
                        continue;

                    module = (ILobbyModule)Activator.CreateInstance(type);
                    break;
                }
                if (module == null)
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
                    continue;
                }

                // Apply Module Patches
                HarmonyLib.Harmony harmony = new(moduleName);
                if (!MelonMain.ApplyPatches(harmony, module.GetType().Assembly, $"[{moduleName.Pastel("#800080")}] "))
                {
#if DEBUG
                    MelonMain._logger.Error($"Failed to Apply Patches for {moduleName}");
#endif
                    harmony.UnpatchSelf();
                    module.OnQuit();
                    continue;
                }

                // Add Module to Cache
                module.HarmonyInstance = harmony;
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
