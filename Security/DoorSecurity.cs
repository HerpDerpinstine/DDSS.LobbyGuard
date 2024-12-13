using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppProps.Door;
using System.Collections;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class DoorSecurity
    {
        private const float _doorAnimationDelay = 2f;

        internal static void DoorStart(DoorController door)
        {
            if ((door == null)
                || door.WasCollected)
                return;

            FixColliderSize(door.playerDetectionVolumeForward);
            FixColliderSize(door.playerDetectionVolumeBackward);

            door.StopAllCoroutines();
            door.StartCoroutine(CheckStateCoroutine(door));
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

        private static void FixColliderSize(PlayerDetectionVolume volume)
        {
            if ((volume == null)
                && volume.WasCollected)
                return;

            BoxCollider collider = volume.GetComponent<BoxCollider>();
            if ((collider == null)
                || collider.WasCollected)
                return;

            Vector3 size = collider.size;
            if (size.x < 0.6f)
                size.x = 0.6f;
            if (size.y < 0.6f)
                size.y = 0.6f;
            if (size.z < 0.6f)
                size.z = 0.6f;
            collider.size = size;
        }

        internal static IEnumerator CheckStateCoroutine(DoorController door)
        {
            // Loop
            while (IsDoorValid(door))
            {
                // Check if Door should be Open
                int volumeState = 0;
                while (IsDoorValid(door)
                    && ((volumeState = GetVolumeState(door)) == 0))
                    yield return null;

                // Open Door
                if (IsDoorValid(door))
                    door.Networkstate = volumeState;

                // Wait for Animation
                yield return new WaitForSeconds(_doorAnimationDelay);

                // Check if Door should be Closed
                while (IsDoorValid(door)
                    && ((volumeState = GetVolumeState(door)) != 0))
                    yield return new WaitForSeconds(1f);

                // Close Door
                if (IsDoorValid(door))
                    door.Networkstate = volumeState;
            }

            yield break;
        }
    }
}
