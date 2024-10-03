using Il2CppGameManagement;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        internal const float MAX_DISTANCE = 2f;
        internal const float MAX_SPANK_DISTANCE = 1f;

        internal const int MAX_CIGS_PER_PACK = 20;
        internal const int MAX_ITEMS_HELD = 2;

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB,
            float maxRange = MAX_DISTANCE)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;
            return distance <= maxRange;
        }

        internal static bool CanSpawnItem(string interactableName, int maxCount)
        {
            if (GameManager.instance == null)
                return false;

            if (!GameManager.instance.collectibleCounts.ContainsKey(interactableName))
                GameManager.instance.collectibleCounts[interactableName] = 0;

           return GameManager.instance.CountSpawnedItemsOfType(interactableName) < maxCount;
        }

        internal static bool CanGrabCollectible(LobbyPlayer player)
        {
            PlayerController controller = player.NetworkplayerController.GetComponent<PlayerController>();
            if (controller == null)
                return false;

            return controller.currentUsables.Count < MAX_ITEMS_HELD;
        }

        internal static bool CanGrabCollectible(LobbyPlayer player, Collectible collectible)
        {
            PlayerController controller = player.NetworkplayerController.GetComponent<PlayerController>();
            if (controller == null)
                return false;

            int count = controller.currentUsables.Count;
            return ((count < MAX_ITEMS_HELD) 
                && (count < collectible.maxStack));
        }

        internal static bool IsHoldingCollectible(LobbyPlayer player)
        {
            PlayerController controller = player.NetworkplayerController.GetComponent<PlayerController>();
            if (controller == null)
                return false;

            return controller.currentUsables.Count > 0;
        }
    }
}
