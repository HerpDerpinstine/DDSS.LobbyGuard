using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.TrashBin;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class TrashBinSecurity
    {
        private const float FIRE_DELAY = 4f;
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

            // Validate Role
            if (state)
            {
                LobbyPlayer lobbyplayer = sender.GetComponent<LobbyPlayer>();
                if ((lobbyplayer == null)
                    || lobbyplayer.WasCollected
                    || lobbyplayer.NetworkisFired)
                    return;
            }

            // Cache New Coroutine
            _enableFireCoroutines[trashcan] =
                trashcan.StartCoroutine(
                    trashcan.DelayedEnableFire(sender, state, state ? FIRE_DELAY : 0f));
        }
    }
}
