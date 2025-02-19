using MelonLoader;
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
        public static string _modulePath { get; private set; }
        private static List<ILobbyModule> _modules = new();

        internal static void Load()
        {
            // Setup Modules Folder
            _modulePath = Path.Combine(MelonMain._userDataPath, "Modules");
            if (!Directory.Exists(_modulePath))
                Directory.CreateDirectory(_modulePath);

            List<ILobbyModule> validModules = new();
            int moduleFailures = 0;
            foreach (var filePath in Directory.GetFiles(_modulePath, "*.dll", SearchOption.AllDirectories))
            {
                // Load Assembly
                Assembly asm = null;
                try
                {
                    asm = AssemblyLoadContext.Default.LoadFromAssemblyPath(filePath);
                }
                catch (Exception ex)
                {
                    moduleFailures++;
                    MelonMain._logger.Error($"Exception while attempting to load {filePath}: {ex}");
                    continue;
                }
                if (asm == null)
                {
                    moduleFailures++;
                    MelonMain._logger.Error($"Failed to load {filePath}");
                    continue;
                }

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

                // Add Module to Cache
                validModules.Add(module);
            }

            // Sort Modules by Priority
            validModules = validModules.OrderBy(item => item.Priority).ToList();

            // Run OnLoad
            foreach (var module in validModules)
            {
                // Run OnLoad
                if (!module.OnLoad())
                {
                    module.OnQuit();
                    continue;
                }

                // Apply Module Patches
                HarmonyLib.Harmony harmony = new(module.Name);
                if (!MelonMain.ApplyPatches(harmony, module.GetType().Assembly))
                {
                    harmony.UnpatchSelf();
                    module.OnQuit();
                    continue;
                }

                // Add Module to Cache
                module.HarmonyInstance = harmony;
                _modules.Add(module);
                MelonMain._logger.Msg($"Module Loaded: {module.Name}");
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
