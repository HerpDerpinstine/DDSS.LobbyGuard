using Il2Cpp;
using Il2CppGameManagement;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppProps.Easel;
using Il2CppProps.Printer;
using Il2CppProps.WorkStation.Mouse;
using UnityEngine;

namespace DDSS_LobbyGuard.SecurityExtension
{
    internal static class InteractionSecurity
    {
        private static readonly Il2CppSystem.Type DocumentType = Il2CppType.Of<Document>();
        private static readonly Il2CppSystem.Type ImageType = Il2CppType.Of<PrintedImage>();
        private static readonly Il2CppSystem.Type StorageBoxType = Il2CppType.Of<StorageBox>();
        private static readonly Il2CppSystem.Type ToiletPaperType = Il2CppType.Of<ToiletPaper>();
        private static readonly Il2CppSystem.Type MouseType = Il2CppType.Of<Mouse>();

        // These types use MAX_DISTANCE_EXTENDED for their Range Checks
        // Be sure to apply these additionally in
        // `DDSS_LobbyGuard.Modules.Security.Object.Patches`
        // `Patch_Interactable.InvokeUserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient_Prefix`
        internal static readonly Il2CppSystem.Type TVType = Il2CppType.Of<TVController>();
        internal static readonly Il2CppSystem.Type CCTVType = Il2CppType.Of<CCTVController>();
        internal static readonly Il2CppSystem.Type EaselType = Il2CppType.Of<EaselController>();
        internal static readonly Il2CppSystem.Type WhiteBoardType = Il2CppType.Of<WhiteBoardController>();
        internal static readonly Il2CppSystem.Type KitchenCabinetType = Il2CppType.Of<KitchenCabinetController>();

        internal const float MAX_DISTANCE_DEFAULT = 2f;
        internal const float MAX_DISTANCE_EXTENDED = 3f;

        internal const int MAX_DOCUMENTS_TRAY = 10;
        internal const int MAX_DOCUMENTS_BINDER = 10;

        internal const int MAX_STICKYNOTES_PLAYER = 1;
        internal const int MAX_STICKYNOTES_DOOR = 10;
        internal const int MAX_COLLECTIBLES_HOLDER = 10;

        internal const int MAX_CHAT_CHARS = 50;
        internal const int MAX_STICKYNOTE_CHARS = 100;

        internal const int MAX_DOCUMENT_CHARS = 240;

        internal const int MAX_INTERACTION_COOLDOWN = 30;

        internal static int MAX_CIGS { get; private set; }
        internal static int MAX_CIG_PACKS { get; private set; }
        internal static int MAX_INFECTED_USBS { get; private set; }
        internal static int MAX_WATERCUPS { get; private set; }

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
            MAX_WATERCUPS = MAX_PLAYERS * 2;
            MAX_CIGS = MAX_PLAYERS * 3;
        }

        internal static bool IsWithinRange(Vector3 posA, Vector3 posB,
            float maxRange = MAX_DISTANCE_DEFAULT)
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

        internal static bool CanUseUsable(NetworkIdentity player, Usable usable)
        {
            if ((player == null)
                || player.WasCollected)
                return false;

            // Get PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Check Total Count
            bool isChair = false;
            if ((usable != null)
                && !usable.WasCollected)
            {
                isChair = usable.TryCast<Chair>();
                if (!isChair)
                {
                    var usableType = usable.GetIl2CppType();
                    bool hasOtherType = false;
                    foreach (var otherUsable in controller.currentUsables)
                        if ((otherUsable != null)
                            && !otherUsable.WasCollected)
                        {
                            Il2CppSystem.Type otherUsableType = otherUsable.GetIl2CppType();
                            if (otherUsableType != usableType)
                            {
                                hasOtherType = true;
                                break;
                            }
                        }
                    if (hasOtherType)
                        return false;

                    int count = 0;
                    if (controller.currentUsables != null)
                        count = controller.currentUsables.Count;
                    if (count < 0)
                        count = 0;

                    int maxAmount = 1;
                    if ((usableType == DocumentType)
                        || (usableType == ImageType)
                        || (usableType == StorageBoxType)
                        || (usableType == ToiletPaperType)
                        || (usableType == MouseType))
                        maxAmount = 2;

                    if (count >= maxAmount)
                        return false;
                }
            }

            // Allow Grab
            return true;
        }

        internal static bool CanStopUseUsable(NetworkIdentity player, bool isChair)
        {
            // Get PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Allow Drop
            return true;
        }
    }
}
