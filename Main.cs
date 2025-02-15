﻿using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.GUI;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
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
        internal static bool _errorOccured;
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
            CollectibleDestructionCallback.Register();

            // Apply Patches
            ApplyPatches();
            MakeModHelperAware();

            // Prevent Exceptions from causing Disconnects
            NetworkServer.exceptionsDisconnect = false;
            NetworkClient.exceptionsDisconnect = false;

            // Log Success
            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if ((sceneName == "MainMenuScene")
                || (sceneName == "LobbyScene"))
            {
                DoorSecurity.OnSceneLoad();
                TrashBinSecurity.OnSceneLoad();
                CollectibleSecurity.OnSceneLoad();
                InteractionSecurity.OnSceneLoad();
                PlayerTriggerSecurity.OnSceneLoad();
                InteractionSecurity.UpdateSettings();
                ComputerSecurity._playerAddresses.Clear();
            }

            if (sceneName == "MainMenuScene") // Main Menu
            {
                LobbySecurity.OnSceneLoad();
                MainMenuPanelBuilder.MainMenuInit();

                if (_firstMenuLoad)
                {
                    _firstMenuLoad = false;
                    VersionCheck.Run();
                }
            }
        }

        internal static void InitErrorPrompt()
        {
            if (!ConfigHandler.General.PromptForInitializationError.Value
                || !_errorOccured)
            {
                BlacklistErrorPrompt();
                return;
            }

            try
            {
                ShowPrompt("ERROR",
                    "LobbyGuard encountered Errors during Initialization!\nThis might cause instability and/or crashing.\nContinue?",
                    "Play Game",
                    "Quit",
                    new Action(BlacklistErrorPrompt),
                    new Action(Application.Quit),
                    new Action(BlacklistErrorPrompt));
            }
            catch (Exception ex)
            {
                _errorOccured = true;
                _logger.Error(ex);
                BlacklistErrorPrompt();
            }
        }

        internal static void BlacklistErrorPrompt()
        {
            if (!ConfigHandler.General.PromptForBlacklistError.Value
                || !BlacklistSecurity._error)
                return;

            try
            {
                ShowPrompt("ERROR",
                    $"LobbyGuard encountered an Error while {(BlacklistSecurity._errorSave ? "Saving" : "Loading")} Blacklist.json\nDisabling Persistent Blacklist until Error is Resolved.\nContinue?",
                    "Play Game",
                    "Quit",
                    new Action(() => { }),
                    new Action(Application.Quit),
                    new Action(() => { }));
            }
            catch (Exception ex)
            {
                _errorOccured = true;
                _logger.Error(ex);
            }
        }

        internal static void ShowPrompt(
            string title,
            string content,
            string confirmText, 
            string cancelText, 
            Action confirmAction, 
            Action cancelAction,
            Action errorAction = null)
        {
            if ((UIManager.instance == null)
                || UIManager.instance.WasCollected)
            {
                _errorOccured = true;
                _logger.Error(new NullReferenceException("UIManager.instance"));
                if (errorAction != null)
                    errorAction();
                return;
            }

            try
            {
                UIManager.instance.ShowPrompt(title, content, confirmText, cancelText, confirmAction, cancelAction);
            }
            catch (Exception ex)
            {
                _errorOccured = true;
                _logger.Error(ex);
                if (errorAction != null)
                    errorAction();
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
                    _errorOccured = true;
                    LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
                }
            }
        }

        internal void MakeModHelperAware()
        {
            MelonMod modHelper = null;
            foreach (var mod in RegisteredMelons)
                if (mod.Info.Name == "ModHelper")
                {
                    modHelper = mod;
                    break;
                }
            if (modHelper == null)
                return;

            Type modFilterType = modHelper.MelonAssembly.Assembly.GetType("DDSS_ModHelper.Utils.RequirementFilter");
            if (modFilterType == null) 
                return;

            MethodInfo method = modFilterType.GetMethod("AddOptionalMelon", BindingFlags.Public | BindingFlags.Static);
            if (method == null) 
                return;

            method.Invoke(null, [this]);
        }
    }   
}