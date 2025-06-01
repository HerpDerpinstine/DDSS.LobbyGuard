using DDSS_LobbyGuard.Utils;
using Il2CppUMUI;
using System;
using System.Collections;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    internal static class ModSettingsManager
    {
        internal static readonly string _keyCodePrefix = "KeyCode_";
        internal static readonly int _keyCodePrefixLen = _keyCodePrefix.Length;

        internal static ModSettingsBuilder _builder;
        internal static Coroutine _rebindCoroutine;
        private static Action _lastOnCancel;

        internal static void SceneInit(string sceneName)
        {
            switch (sceneName)
            {
                case "MainMenuScene":
                    _builder = new MainMenuModSettings();
                    break;

                default:
                    break;
            }
        }

        internal static void OpenModSettings()
        {
            // Validate
            if (UIManager.instance == null
                || UIManager.instance.WasCollected)
                return;

            // Open Custom Tab
            UIManager.instance.tabs["LobbyGuardSettings"] = _builder._tab;
            UIManager.instance.OpenTab("LobbyGuardSettings");
        }

        internal static float IncrementCustomSettingValue(Type targetType,
            int inc,
            float value,
            string minValueStr,
            string maxValueStr)
        {
            CancelRebind();

            // Get Decimal Types to Handle Differently
            Type floatType = typeof(float);

            // Check Target for Decimal Type
            bool isDecimal = false;
            if (targetType == floatType)
                isDecimal = true;

            // Get Additive Value
            float additive = 1f;
            if (isDecimal)
                additive = 0.1f;

            // Apply Additive
            value += additive * inc;
            value = (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);

            // Get Min and Max Values
            float minValue = (float)Math.Round(float.Parse(minValueStr), 1, MidpointRounding.AwayFromZero);
            float maxValue = (float)Math.Round(float.Parse(maxValueStr), 1, MidpointRounding.AwayFromZero);

            // Roll-Over if Outside of Bounds
            if (value > maxValue)
                value = additive * (inc - 1);
            if (value < minValue)
                value = maxValue - additive * (inc - 1);

            return (float)Math.Round(value, 1, MidpointRounding.AwayFromZero);
        }

        internal static void StartRebind(Action<KeyCode> onChange,
            Action onCancel,
            Action<int> onTick,
            int maxTime = 5)
        {
            if (_rebindCoroutine != null)
                CancelRebind();

            _lastOnCancel = onCancel;
            _rebindCoroutine =
                _builder._tab.StartCoroutine(RebindCoroutine(onChange, onCancel, onTick, maxTime));
        }

        internal static void CancelRebind()
        {
            if (_rebindCoroutine == null)
                return;
            _builder._tab.StopCoroutine(_rebindCoroutine);
            _lastOnCancel();
            _lastOnCancel = null;
            _rebindCoroutine = null;
        }

        private static IEnumerator RebindCoroutine(Action<KeyCode> onChange,
            Action onCancel,
            Action<int> onTick,
            int maxTime)
        {
            DateTime startTime = DateTime.Now;
            KeyCode newCode = KeyCode.None;
            int secondsSpent = 0;
            bool success;
            while (!(success = CheckForKeyPress(startTime, maxTime, ref secondsSpent, ref newCode)))
            {
                onTick(maxTime - secondsSpent);
                yield return null;
            }

            if (!success)
                onCancel();
            else
                onChange(newCode);

            _rebindCoroutine = null;
            _lastOnCancel = null;

            yield break;
        }

        private static bool CheckForKeyPress(DateTime startTime,
            int maxTime,
            ref int secondsSpent,
            ref KeyCode newCode)
        {
            secondsSpent = (int)(DateTime.Now - startTime).TotalSeconds;
            if (secondsSpent > maxTime)
                return true;

            bool foundKey = false;
            KeyCode[] values = Enum.GetValues<KeyCode>();
            foreach (KeyCode keyCode in values)
            {
                if ((int)keyCode > (int)KeyCode.Menu)
                    break;

                foundKey = Input.GetKeyDown(keyCode);
                if (foundKey)
                {
                    newCode = keyCode;
                    break;
                }
            }

            return foundKey;
        }
    }
}
