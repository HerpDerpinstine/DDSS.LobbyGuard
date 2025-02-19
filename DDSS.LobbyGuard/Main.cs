using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Config;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
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
    public class MelonMain : MelonMod
    {
        public static string _userDataPath { get; private set; }
        public static MelonLogger.Instance _logger { get; private set; }
        public static HarmonyLib.Harmony _harmony { get; private set; }

        private bool _hasError;

        public override void OnInitializeMelon()
        {
            // Cache Logger 
            _logger = LoggerInstance;

            // Cache Harmony
            _harmony = HarmonyInstance;

            // Setup UserData Folder
            _userDataPath = Path.Combine(MelonEnvironment.UserDataDirectory, Properties.BuildInfo.Name);
            if (!Directory.Exists(_userDataPath))
                Directory.CreateDirectory(_userDataPath);

            // Prevent Exceptions from causing Disconnects
            NetworkServer.exceptionsDisconnect = false;
            NetworkClient.exceptionsDisconnect = false;

            // Let ModHelper know this isn't required for everyone
            MakeModHelperAware();

            // Register Main Custom Components
            if (!ManagedEnumerator.Register())
            {
                _hasError = true;
                return;
            }

            // Apply Main Patches
            Assembly melonAssembly = typeof(MelonMain).Assembly;
            if (!ApplyPatches(_harmony, melonAssembly))
            {
                _hasError = true;
                _harmony.UnpatchSelf();
                return;
            }

            // Load Modules
            LobbyModuleManager.Load();

            // Log Success
            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (_hasError)
                return;

            LobbyModuleManager.SceneInit(buildIndex, sceneName);
        }

        public override void OnApplicationQuit()
        {
            if (_hasError)
                return;

            // Remove Modules
            LobbyModuleManager.Quit();
        }

        public static bool RegisterComponent<T>(params Type[] interfaces)
            where T : class
        {
            try
            {
                ClassInjector.RegisterTypeInIl2Cpp<T>(new()
                {
                    LogSuccess = true,
                    Interfaces = interfaces
                });
            }
            catch (Exception e)
            {
                _logger.Error($"Exception while attempting to Register {typeof(T).Name}: {e}");
                return false;
            }
            return true;
        }

        public static bool ApplyPatches(HarmonyLib.Harmony harmonyInstance, Assembly assembly, string prefix = "")
        {
            foreach (Type type in assembly.GetValidTypes())
            {
                // Check Type for any Harmony Attribute
                if (type.GetCustomAttribute<HarmonyPatch>() == null)
                    continue;

                // Apply
                try
                {
#if DEBUG
                    _logger.Msg($"{prefix}Applying {type.Name}");
#endif

                    harmonyInstance.PatchAll(type);
                }
                catch (Exception e)
                {
                    _logger.Error($"Exception while attempting to apply {type.Name}: {e}");
                    return false;
                }
            }
            return true;
        }

        private void MakeModHelperAware()
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

        public static bool ShowPrompt(
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
                _logger.Error(new NullReferenceException("UIManager.instance"));
                if (errorAction != null)
                    errorAction();
                return false;
            }

            try
            {
                UIManager.instance.ShowPrompt(title, content, confirmText, cancelText, confirmAction, cancelAction);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                if (errorAction != null)
                    errorAction();
                return false;
            }

            return true;
        }

        public static bool IsWithinRange(Vector3 posA, Vector3 posB,
            float maxRange)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;

            return distance <= maxRange;
        }
    }
}