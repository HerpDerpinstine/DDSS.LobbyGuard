using DDSS_LobbyGuard.Utils;
using Il2CppProps.Door;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class PlayerTriggerSecurity
    {
        private static Dictionary<PlayerDetectionVolume, int> _playerCounts = new();

        internal static void OnSceneLoad()
            => _playerCounts.Clear();

        internal static bool IsColliderValid(Collider other)
            => ((other.gameObject.layer != 11) && (other.gameObject.layer != 13));

        internal static void OnEnter(PlayerDetectionVolume trigger, Collider other)
        {
            int count = 0;
            _playerCounts.TryGetValue(trigger, out count);
            if (count <= 0)
                count = 0;
            _playerCounts[trigger] = count++;
            trigger.isPlayerInside = true;
            trigger.StartCoroutine(ColliderCheckCoroutine(trigger, other));
        }

        private static IEnumerator ColliderCheckCoroutine(PlayerDetectionVolume trigger, Collider other)
        {
            Collider triggerCollider = trigger.GetComponent<Collider>();

            while (other.bounds.Contains(triggerCollider.bounds.min)
                || other.bounds.Contains(triggerCollider.bounds.max)
                || triggerCollider.bounds.Contains(other.bounds.min)
                || triggerCollider.bounds.Contains(other.bounds.max))
                yield return new WaitForSeconds(1f);

            trigger.OnPlayerExit.Invoke();

            int count = 0;
            _playerCounts.TryGetValue(trigger, out count);
            _playerCounts[trigger] = count--;
            if (count <= 0)
            {
                count = 0;
                trigger.isPlayerInside = false;
            }
        }
    }
}
