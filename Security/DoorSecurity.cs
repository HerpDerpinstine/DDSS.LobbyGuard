using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class DoorSecurity
    {
        private const float _doorCloseInitialDelayMs = 2f;
        private const float _doorCloseFinalDelayMs = 1f;

        private static Dictionary<DoorController, Coroutine> _doorStateCoroutines = new();

        internal static void OnSceneLoad()
            => _doorStateCoroutines.Clear();

        internal static void ApplyState(DoorController door, int newState)
        {
            if (_doorStateCoroutines.ContainsKey(door))
                return;
            door.StopAllCoroutines();
            _doorStateCoroutines[door] =
                door.StartCoroutine(ApplyStateCoroutine(door, newState));
        }

        private static IEnumerator ApplyStateCoroutine(DoorController door, int newState)
        {
            // Apply Open State
            door.Networkstate = newState;
            yield return new WaitForSeconds(_doorCloseInitialDelayMs);

            // Wait for Player to get out of the way
            while (door.playerDetectionVolumeBackward.isPlayerInside
                || door.playerDetectionVolumeForward.isPlayerInside)
                yield return null;

            // Apply Closed State
            yield return new WaitForSeconds(_doorCloseFinalDelayMs);
            if (door.playerDetectionVolumeBackward.isPlayerInside
                || door.playerDetectionVolumeForward.isPlayerInside)
                yield return ApplyStateCoroutine(door, newState);
            else
            {
                door.Networkstate = 0;

                // Remove Coroutine from Cache
                if (_doorStateCoroutines.ContainsKey(door))
                    _doorStateCoroutines.Remove(door);
            }
        }
    }
}
