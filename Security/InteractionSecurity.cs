using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.StateMachineLogic;
using Il2CppTMPro;
using Il2CppUI.Tabs.LobbyTab;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class InteractionSecurity
    {
        private static Dictionary<LobbyPlayer, bool> _allSlackers = new();
        private static Dictionary<PlayerLobbyUI, TextMeshProUGUI> _allCharacterNames = new();

        internal const float MAX_DISTANCE_DEFAULT = 2f;

        internal const float MAX_DISTANCE_PLAYER = 1f;
        internal const float MAX_DISTANCE_CCTV = 3f;

        internal const int MAX_DOCUMENTS_TRAY = 10;
        internal const int MAX_DOCUMENTS_BINDER = 10;

        internal const int MAX_STICKYNOTES_PLAYER = 1;
        internal const int MAX_STICKYNOTES_DOOR = 10;
        internal const int MAX_COLLECTIBLES_HOLDER = 10;

        internal const int MAX_CHAT_CHARS = 50;
        internal const int MAX_STICKYNOTE_CHARS = 100;

        internal const int MAX_DOCUMENT_CHARS = 240;

        internal static int MAX_CIGS { get; private set; }
        internal static int MAX_CIG_PACKS { get; private set; }
        internal static int MAX_INFECTED_USBS { get; private set; }

        internal static int MAX_PLAYERS { get; private set; }

        internal static void OnSceneLoad()
        {
            _allSlackers.Clear();
            _allCharacterNames.Clear();
        }

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

        internal static bool IsSlacker(LobbyPlayer player)
        {
            if (player == null)
                return false;
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return player.NetworkplayerRole == PlayerRole.Slacker;
            return _allSlackers.ContainsKey(player);
        }

        internal static void AddSlacker(LobbyPlayer player)
        {
            if (player == null)
                return;
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return;
            _allSlackers[player] = true;
        }

        internal static void RemoveSlacker(LobbyPlayer player)
        {
            if (player == null)
                return;
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return;
            if (!_allSlackers.ContainsKey(player))
                return;
            _allSlackers.Remove(player);
        }

        internal static void AddLobbyUICharacterName(PlayerLobbyUI ui, TextMeshProUGUI text)
            => _allCharacterNames[ui] = text;

        internal static TextMeshProUGUI GetLobbyUICharacterName(PlayerLobbyUI ui)
        {
            if (_allCharacterNames.TryGetValue(ui, out var text))
                return text;
            return null;
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

        internal static PlayerRole GetWinner(GameManager manager)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return manager.GetWinner();

            if (manager.onlyWinFromScore)
                return PlayerRole.None;

            if (manager.finalProductivityMeter >= 1f)
                return PlayerRole.Specialist;

            if (manager.finalProductivityMeter <= 0f)
                return PlayerRole.Slacker;

            int numberOfFiredEmployees = manager.GetNumberOfFiredEmployees();
            int amountOfUnfiredSlackers = GetAmountOfUnfiredSlackers(LobbyManager.instance);
            int amountOfUnfiredSpecialists = GetAmountOfUnfiredSpecialists(LobbyManager.instance);

            if (amountOfUnfiredSlackers <= 0)
                return PlayerRole.Specialist;

            if (amountOfUnfiredSpecialists <= 0)
                return PlayerRole.Slacker;

            if (numberOfFiredEmployees >= manager.fireLimit)
                return PlayerRole.Slacker;

            return PlayerRole.None;

        }

        internal static int GetAmountOfUnfiredSlackers(LobbyManager manager)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return manager.GetAmountOfUnfiredSlackers();

            int num = 0;
            foreach (NetworkIdentity networkIdentity in manager.connectedLobbyPlayers)
            {
                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if (IsSlacker(player)
                    && !player.isFired)
                    num++;
            }
            return num;
        }

        internal static int GetAmountOfUnfiredSpecialists(LobbyManager manager)
        {
            if (!ConfigHandler.Gameplay.HideSlackersFromClients.Value)
                return manager.GetAmountOfUnfiredSpecialists();

            int num = 0;
            foreach (NetworkIdentity networkIdentity in manager.connectedLobbyPlayers)
            {
                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if ((player.NetworkplayerRole == PlayerRole.Specialist)
                    && !IsSlacker(player)
                    && !player.isFired)
                    num++;
            }
            return num;
        }

        internal static bool CanUseUsable(NetworkIdentity player, bool isChair)
        {
            // Get PlayerController
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Check for Point or Hand Raise
            if (!isChair
                && !ConfigHandler.Gameplay.GrabbingWhilePointing.Value
                && (controller.IsPointing() || controller.IsRaisingHand()))
                return false;

            // Check for Handshake
            if ((isChair || !ConfigHandler.Gameplay.GrabbingWhileHandshaking.Value)
                && controller.IsHandShaking())
                return false;

            // Check for Emotes
            if (controller.IsEmoting()
                && ((isChair && !controller.IsClapping() && !controller.IsWaving() && !controller.IsFacePalming())
                    || (!isChair && !ConfigHandler.Gameplay.GrabbingWhileEmoting.Value)))
                return false;

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

            // Check for Point or Hand Raise
            if (!isChair
                && !ConfigHandler.Gameplay.DroppingWhilePointing.Value
                && (controller.IsPointing() || controller.IsRaisingHand()))
                return false;

            // Check for Handshake
            if ((isChair || !ConfigHandler.Gameplay.DroppingWhileHandshaking.Value)
                && controller.IsHandShaking())
                return false;

            // Check for Emotes
            if (controller.IsEmoting()
                && ((isChair && !controller.IsClapping() && !controller.IsWaving() && !controller.IsFacePalming())
                    || (!isChair && !ConfigHandler.Gameplay.DroppingWhileEmoting.Value)))
                return false;

            // Allow Drop
            return true;
        }
    }
}
