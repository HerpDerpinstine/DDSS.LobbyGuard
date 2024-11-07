using Il2Cpp;
using Il2CppGameManagement;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        internal const float MAX_DISTANCE = 2f;
        internal const float MAX_SPANK_DISTANCE = 1f;

        internal const int MAX_DOCUMENTS_TRAY = 10;
        internal const int MAX_DOCUMENTS_BINDER = 10;

        internal const int MAX_STICKYNOTES_PLAYER = 1;
        internal const int MAX_STICKYNOTES_DOOR = 10;
        internal const int MAX_COLLECTIBLES_HOLDER = 10;

        internal const int MAX_CHAT_CHARS = 50;
        internal const int MAX_STICKYNOTE_CHARS = 100;

        internal const int MAX_DOCUMENT_CHARS = 100;

        internal static int MAX_CIGS { get; private set; }
        internal static int MAX_CIG_PACKS { get; private set; }
        internal static int MAX_INFECTED_USBS { get; private set; }

        internal static int MAX_PLAYERS { get; private set; }

        internal static void UpdateSettings()
        {
            // Validate Game Rules Manager
            if (GameRulesSettingsManager.instance == null)
                return;

            // Get Max Players
            MAX_PLAYERS = Mathf.RoundToInt(GameRulesSettingsManager.instance.GetSetting("Max players")) + 1;

            // Adjust Limits
            MAX_CIG_PACKS = MAX_PLAYERS;
            MAX_INFECTED_USBS = MAX_PLAYERS;
            MAX_CIGS = MAX_PLAYERS * 3;
        }

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB,
            float maxRange = MAX_DISTANCE)
        {
            float distance = Vector3.Distance(posA, posB);
            if (distance < 0f)
                distance *= -1f;
            return distance <= maxRange;
        }

        private static int GetTotalCountOfSpawnedItem(string interactableName)
        {
            if (GameManager.instance == null)
                return 0;

            return GameManager.instance.CountSpawnedItemsOfType(interactableName);
        }
        internal static bool CanSpawnItem(string interactableName, int maxCount)
            => GetTotalCountOfSpawnedItem(interactableName) < maxCount;
    }
}
