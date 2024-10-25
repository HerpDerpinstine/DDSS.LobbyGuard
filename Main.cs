using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.GUI;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using System.Reflection;

namespace DDSS_LobbyGuard
{
    internal class MelonMain : MelonMod
    {
        internal static string _userDataPath;
        internal static MelonLogger.Instance _logger;

        public override void OnInitializeMelon()
        {
            // Cache Logger 
            _logger = LoggerInstance;

            // Setup UserData Folder
            _userDataPath = Path.Combine(MelonEnvironment.UserDataDirectory, Properties.BuildInfo.Name);
            if (!Directory.Exists(_userDataPath))
                Directory.CreateDirectory(_userDataPath);

            // Setup Config
            ConfigHandler.Setup();
            BlacklistSecurity.Setup();

            // Register Custom Components
            ManagedEnumerator.Register();
            KeyDestructionCallback.Register();
            FireExDestructionCallback.Register();

            // Apply Patches
            ApplyPatches();

            // Log Success
            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            DoorSecurity.OnSceneLoad();
            PhoneSecurity.OnSceneLoad();
            InteractionSecurity.UpdateSettings();
            ServerSecurity.OnSceneLoad();
            TrashBinSecurity.OnSceneLoad();
            StereoSecurity.OnSceneLoad();
            KeySecurity.OnSceneLoad();

            if (sceneName == "MainMenuScene") // Main Menu
            {
                MainMenuPanelBuilder.MainMenuInit();
            }
        }

        private void ApplyPatches()
        {
            Assembly melonAssembly = typeof(MelonMain).Assembly;
            foreach (Type type in melonAssembly.GetValidTypes())
            {
                // Check Type for any Harmony Attribute
                if (type.GetCustomAttribute<HarmonyPatch>() == null)
                    continue;

                // Apply
                try
                {
                    if (MelonDebug.IsEnabled())
                        LoggerInstance.Msg($"Applying {type.Name}");

                    HarmonyInstance.PatchAll(type);
                }
                catch (Exception e)
                {
                    LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
                }
            }
        }
    }   
}