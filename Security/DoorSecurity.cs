﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppProps.Door;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
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

        internal static void DoorStart()
        {
            foreach (var volume in Object.FindObjectsByType<PlayerDetectionVolume>(FindObjectsSortMode.None))
            {
                if ((volume == null)
                    || volume.WasCollected)
                    continue;
                FixColliderSize(volume);
            }
        }

        internal static void ApplyState(DoorController door, int newState)
        {
            if (_applyStateCoroutines.ContainsKey(door))
                door.StopCoroutine(_applyStateCoroutines[door]);

            FixColliderSize(door.playerDetectionVolumeForward);
            FixColliderSize(door.playerDetectionVolumeBackward);

            _applyStateCoroutines[door] = 
                door.StartCoroutine(ApplyStateCoroutine(door, newState));
        }

        internal static void FixColliderSize(PlayerDetectionVolume volume)
        {
            if ((volume == null)
                || volume.WasCollected)
                return;

            BoxCollider collider = volume.GetComponent<BoxCollider>();
            if ((collider == null)
                || collider.WasCollected)
                return;

            Vector3 size = collider.size;
            if (size.x < 0.5f)
                size.x = 0.5f;
            if (size.y < 0.5f)
                size.y = 0.5f;
            if (size.z < 0.5f)
                size.z = 0.5f;
            collider.size = size;
        }

        private static bool IsDoorValid(DoorController door)
        {
            if ((door == null)
                || door.WasCollected)
                return false;
            return true;
        }

        private static bool GetLockState(DoorController door)
        {
            if (!ConfigHandler.Gameplay.CloseDoorsOnLock.Value
                || !IsDoorValid(door))
                return false;
            return door.NetworkisLocked;
        }

        private static int GetVolumeState(DoorController door)
        {
            if (!IsDoorValid(door)
                || GetLockState(door))
                return 0;

            if ((door.playerDetectionVolumeForward != null)
                && !door.playerDetectionVolumeForward.WasCollected
                && door.playerDetectionVolumeForward.isPlayerInside)
                return 1;

            if ((door.playerDetectionVolumeBackward != null)
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
            for (int i = 0; i < (_doorAnimationDelay / 0.1f); i++)
            {
                if (GetLockState(door))
                    break;
                else
                    yield return new WaitForSeconds(0.1f);
            }

            // Check if Door should be Closed
            int volumeState = 0;
            while (IsDoorValid(door)
                && ((volumeState = GetVolumeState(door)) != 0))
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
