using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.GUI;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppUMUI;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DDSS_LobbyGuard
{
    internal class MelonMain : MelonMod
    {
        internal static string _userDataPath;
        internal static MelonLogger.Instance _logger;
        private static bool _errorOccured;
        private static bool _firstMenuLoad = true;

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

            // Setup Blacklist
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
            KeySecurity.OnSceneLoad();

            if (sceneName == "MainMenuScene") // Main Menu
            {
                MainMenuPanelBuilder.MainMenuInit();

                if (_firstMenuLoad)
                {
                    _firstMenuLoad = false;

                    if (_errorOccured)
                        ShowPrompt("LobbyGuard Error",
                            "Some errors have occured during Initialization.\nLobbyGuard might not function as intended.\nContinue?",
                            "Play Game",
                            "Quit",
                            new Action(VersionCheck.Run),
                            new Action(Application.Quit));
                    else
                        VersionCheck.Run();
                }
            }
        }

        public static void ShowPrompt(string title, string content, string confirmText, string cancelText, Action confirmAction, Action cancelAction)
        {
            if ((UIManager.instance == null)
                || UIManager.instance.WasCollected)
                return;

            UIManager.instance.ShowPrompt(title, content, confirmText, cancelText, confirmAction, cancelAction);
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
                    _errorOccured = true;
                    LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
                }
            }
        }
    }   
}