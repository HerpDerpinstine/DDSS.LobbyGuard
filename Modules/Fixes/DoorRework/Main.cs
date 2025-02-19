using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppProps.Door;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.DoorRework
{
    internal class ModuleMain : ILobbyModule
    {
        private const float _colliderSizeMin = 0.6f;

        private const float _doorAnimationDelay = 2f;
        private static Dictionary<DoorController, Coroutine> _applyStateCoroutines = new();

        public override string Name => "DoorRework";
        public override Type ConfigType => typeof(ModuleConfig);

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;
            _applyStateCoroutines.Clear();
        }

        internal static void DoorStart()
        {
            foreach (var volume in UnityEngine.Object.FindObjectsByType<PlayerDetectionVolume>(FindObjectsSortMode.None))
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
            if (size.x < _colliderSizeMin)
                size.x = _colliderSizeMin;
            if (size.y < _colliderSizeMin)
                size.y = _colliderSizeMin;
            if (size.z < _colliderSizeMin)
                size.z = _colliderSizeMin;
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
            if (!ModuleConfig.Instance.CloseDoorsOnLock.Value
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