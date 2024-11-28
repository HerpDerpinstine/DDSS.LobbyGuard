using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppProps.Door;
using System.Collections;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class DoorSecurity
    {
        private const float _doorCloseDelay = 2f;

        internal static void ApplyState(DoorController door, int newState)
        {
            if ((door == null)
                || door.WasCollected)
                return;

            door.StopAllCoroutines();
            door.StartCoroutine(ApplyStateCoroutine(door, newState));
        }

        private static IEnumerator ApplyStateCoroutine(DoorController door, int newState)
        {
            if ((door == null)
                || door.WasCollected)
                yield break;

            // Apply Open State
            door.Networkstate = newState;
            yield return new WaitForSeconds(_doorCloseDelay);

            // Wait for Player to get out of the way
            while ((door != null)
                && !door.WasCollected
                && (door.playerDetectionVolumeBackward != null)
                && !door.playerDetectionVolumeBackward.WasCollected
                && (door.playerDetectionVolumeForward != null)
                && !door.playerDetectionVolumeForward.WasCollected
                && (door.playerDetectionVolumeBackward.isPlayerInside
                    || door.playerDetectionVolumeForward.isPlayerInside))
                yield return null;

            // Apply Closed State
            if ((door != null)
                || !door.WasCollected)
                door.Networkstate = 0;
        }

        internal static void FixColliderSize(PlayerDetectionVolume volume)
        {
            if ((volume == null)
                && volume.WasCollected)
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
    }
}
