using Il2CppMirror;
using Il2CppProps.TrashBin;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Extras.MoreSlackerSettings.Internal
{
    internal static class TrashBinSecurity
    {
        private static Dictionary<TrashBin, Coroutine> _enableFireCoroutines = new();

        internal static void OnSceneLoad()
        {
            // Clear Cached Coroutines
            _enableFireCoroutines.Clear();
        }

        internal static void OnEnableFireEnd(TrashBin trashcan)
        {
            // Clear Cached Coroutine
            if (_enableFireCoroutines.ContainsKey(trashcan))
                _enableFireCoroutines.Remove(trashcan);
        }

        internal static void OnEnableFireBegin(
            NetworkIdentity sender,
            TrashBin trashcan,
            bool state)
        {
            // Check for Already Running Coroutine
            if (_enableFireCoroutines.ContainsKey(trashcan))
                trashcan.StopCoroutine(_enableFireCoroutines[trashcan]);

            // Cache New Coroutine
            _enableFireCoroutines[trashcan] =
                trashcan.StartCoroutine(
                    trashcan.DelayedEnableFire(sender, state, state ? ModuleConfig.Instance.SlackerTrashBinFireDelay.Value : 0f));
        }
    }
}
