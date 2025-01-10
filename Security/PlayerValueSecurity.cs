using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Security
{
    internal static class PlayerValueSecurity
    {
        private class LobbyPlayerValueCache
        {
            internal PlayerRole role;
            internal PlayerRole originalRole;
            internal SubRole subRole;
            internal WorkStationController workStationController;
            internal NetworkIdentity playerController;
            internal bool isFired;
            internal int colorIndex;
        }
        private static Dictionary<LobbyPlayer, LobbyPlayerValueCache> _allLobbyPlayerValues = new();

        private class PlayerControllerValueCache
        {
            internal LobbyPlayer lobbyPlayer;
        }
        private static Dictionary<PlayerController, PlayerControllerValueCache> _allPlayerControllerValues = new();

        internal static void OnSceneLoad()
        {
            _allLobbyPlayerValues.Clear();
            _allPlayerControllerValues.Clear();
        }

        internal static void Apply(LobbyPlayer player)
        {
            if ((player == null)
                || player.WasCollected)
                return;

            player.NetworkisHost = (player == LobbyManager.instance.GetLocalPlayer());

            if (GameManager.instance.NetworktargetGameState <= (int)GameStates.StartGame)
                return;

            if (player.IsGhost())
                player.NetworkisFired = true;

            if (!_allLobbyPlayerValues.TryGetValue(player, out LobbyPlayerValueCache cache))
                return;

            player.NetworkplayerRole = cache.role;
            player.NetworksubRole = cache.subRole;
            player.NetworkoriginalPlayerRole = cache.originalRole;
            player.NetworkworkStationController = cache.workStationController;
            player.NetworkplayerController = cache.playerController;
            player.NetworkisFired = cache.isFired;
            player.NetworkplayerColorIndex = cache.colorIndex;
        }

        internal static void Apply(PlayerController player)
        {
            if ((player == null)
                || player.WasCollected
                || (GameManager.instance.NetworktargetGameState <= (int)GameStates.StartGame))
                return;

            if (!_allPlayerControllerValues.TryGetValue(player, out PlayerControllerValueCache cache))
                return;

            player.NetworklobbyPlayer = cache.lobbyPlayer;
        }

        internal static PlayerRole GetRole(LobbyPlayer player)
        {
            if ((player == null)
                || player.WasCollected
                || !_allLobbyPlayerValues.TryGetValue(player, out LobbyPlayerValueCache cache))
                return PlayerRole.None;
            return cache.role;
        }
        internal static void SetRole(LobbyPlayer player, PlayerRole role)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.role = role;
        }

        internal static void SetOriginalRole(LobbyPlayer player, PlayerRole role)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.originalRole = role;
        }

        internal static void SetSubRole(LobbyPlayer player, SubRole role)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.subRole = role;
        }

        internal static void SetWorkStationController(LobbyPlayer player, WorkStationController station)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.workStationController = station;
        }

        internal static void SetPlayerController(LobbyPlayer player, NetworkIdentity controller)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.playerController = controller;
        }

        internal static bool GetIsFired(LobbyPlayer player)
        {
            if ((player == null)
                || player.WasCollected
                || !_allLobbyPlayerValues.TryGetValue(player, out LobbyPlayerValueCache cache))
                return false;
            return cache.isFired;
        }

        internal static void SetIsFired(LobbyPlayer player, bool isFired)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.isFired = isFired;
        }

        internal static void SetColorIndex(LobbyPlayer player, int colorIndex)
        {
            if ((player == null)
                || player.WasCollected)
                return;
            LobbyPlayerValueCache cache = null;
            if (!_allLobbyPlayerValues.TryGetValue(player, out cache))
                _allLobbyPlayerValues[player] = cache = new();
            cache.colorIndex = colorIndex;
        }

        internal static void SetLobbyPlayer(PlayerController player, LobbyPlayer lobbyPlayer)
        {
            if ((player == null)
                || player.WasCollected)
                return;

            PlayerControllerValueCache cache = null;
            if (!_allPlayerControllerValues.TryGetValue(player, out cache))
                _allPlayerControllerValues[player] = cache = new();
            cache.lobbyPlayer = lobbyPlayer;
        }
    }
}
