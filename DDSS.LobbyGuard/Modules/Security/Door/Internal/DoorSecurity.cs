using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppProps.Door;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Door.Internal
{
    internal static class DoorSecurity
    {
        private const float _doorAnimationDelay = 2f;
        private static Dictionary<DoorController, Coroutine> _applyStateCoroutines = new();

        internal static void OnSceneLoad()
        {
            // Clear Cached Coroutines
            _applyStateCoroutines.Clear();
        }

        internal static void ApplyState(DoorController door, int newState)
        {
            if (_applyStateCoroutines.ContainsKey(door))
                door.StopCoroutine(_applyStateCoroutines[door]);

            _applyStateCoroutines[door] =
                door.StartCoroutine(ApplyStateCoroutine(door, newState));
        }

        private static bool IsDoorValid(DoorController door)
        {
            if (door == null
                || door.WasCollected)
                return false;
            return true;
        }

        private static bool GetLockState(DoorController door)
        {
            if (!IsDoorValid(door))
                return false;
            return door.NetworkisLocked;
        }

        private static int GetVolumeState(DoorController door)
        {
            if (!IsDoorValid(door)
                || GetLockState(door))
                return 0;

            if (door.playerDetectionVolumeForward != null
                && !door.playerDetectionVolumeForward.WasCollected
                && door.playerDetectionVolumeForward.isPlayerInside)
                return 1;

            if (door.playerDetectionVolumeBackward != null
                && !door.playerDetectionVolumeBackward.WasCollected
                && door.playerDetectionVolumeBackward.isPlayerInside)
                return -1;

            return 0;
        }

        internal static IEnumerator ApplyStateCoroutine(DoorController door, int newState)
        {
            // Open Door
            if (IsDoorValid(door))
                door.Networkstate = newState;

            // Wait for Animation
            for (int i = 0; i < _doorAnimationDelay / 0.1f; i++)
            {
                if (GetLockState(door))
                    break;
                else
                    yield return new WaitForSeconds(0.1f);
            }

            // Check if Door should be Closed
            int volumeState = 0;
            while (IsDoorValid(door)
                && (volumeState = GetVolumeState(door)) != 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    if (GetLockState(door))
                        break;
                    else
                        yield return new WaitForSeconds(0.1f);
                }
            }

            // Close Door
            if (IsDoorValid(door))
                door.Networkstate = volumeState;

            if (_applyStateCoroutines.ContainsKey(door))
                _applyStateCoroutines.Remove(door);

            yield break;
        }
    }
}