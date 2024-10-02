using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        internal const float MAX_DISTANCE = 2f;

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;
            return distance <= MAX_DISTANCE;
        }
    }
}
