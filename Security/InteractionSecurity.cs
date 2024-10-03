using Il2CppGameManagement;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        internal const float MAX_DISTANCE = 2f;
        internal const int MAX_CIGS_PER_PACK = 2;

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;
            return distance <= MAX_DISTANCE;
        }

        internal static bool CanSpawnItem(string interactableName, int maxCount)
        {
            if (GameManager.instance == null)
                return false;

            if (!GameManager.instance.collectibleCounts.ContainsKey(interactableName))
                GameManager.instance.collectibleCounts[interactableName] = 0;

           return GameManager.instance.CountSpawnedItemsOfType(interactableName) < maxCount;
        }
    }
}
