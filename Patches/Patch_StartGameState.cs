using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_StartGameState
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.Enter))]
        private static bool Enter_Prefix(StartGameState __instance)
        {
            // Check if Server
            GameManager manager = __instance.gameManager;
            if (!manager.isServer)
                return false;

            // Get List of Spawn Points
            List<NetworkStartPosition> spawnList = new();
            foreach (NetworkStartPosition spawn in GameObject.FindObjectsByType<NetworkStartPosition>(FindObjectsSortMode.None))
                spawnList.Add(spawn);
            int spawnCount = spawnList.Count;

            // Get List of Workstations
            int workstationCount = manager.workStations.Count;
            List<WorkStationController> workstationList = new();
            foreach (WorkStationController workStationController in manager.workStations)
            {
                // Skip Manager Workstation
                if (workStationController == manager.managerWorkStationController)
                    continue;

                // Add Workstation to List
                workstationList.Add(workStationController);
            }

            // Get List of Players
            int playerCount = LobbyManager.instance.connectedLobbyPlayers.Count;
            List<NetworkIdentity> playerList = new();
            NetworkIdentity playerManager = null;
            foreach (NetworkIdentity player in LobbyManager.instance.connectedLobbyPlayers)
            {
                // Validate Lobby Player
                LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();
                if (lobbyPlayer == null)
                    continue;

                // Check for Manager
                if (lobbyPlayer.playerRole == PlayerRole.Manager)
                {
                    // Cache Manager Player and Skip
                    playerManager = player;
                    continue;
                }

                // Add to List
                playerList.Add(player);
            }

            // Randomize Lists
            workstationList.Shuffle();
            playerList.Shuffle();
            spawnList.Shuffle();

            // Re-Add Manager Player to First in Player List
            if (playerManager != null)
                playerList.Insert(0, playerManager);

            // Iterate through Players
            int slackerCount = 0;
            int specialistCount = 0;
            int slackerAmount = manager.slackerAmount;
            for (int i = 0; i < playerCount; i++)
            {
                // Get Player
                NetworkIdentity player = playerList[i];
                LobbyPlayer lobbyPlayer = player.GetComponent<LobbyPlayer>();

                // Clear Tasks for Player
                player.GetComponent<TaskController>().RpcClearTaskQueue();

                // Move Player to Spawn
                lobbyPlayer.NetworkplayerController.GetComponent<PlayerController>()
                    .UserCode_CmdMovePlayer__Vector3(spawnList[i % spawnCount].transform.position);

                // Assign Player Role and Workstation
                WorkStationController randomWorkstation = workstationList[i % workstationCount];
                if (i == 0) // Manager
                {
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Manager);
                    lobbyPlayer.ServerSetWorkStation(manager.managerWorkStationController, PlayerRole.Manager, true);
                }
                else if (slackerCount <= slackerAmount) // Slacker
                {
                    slackerCount++;
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Slacker);
                    lobbyPlayer.ServerSetWorkStation(randomWorkstation, PlayerRole.Slacker, true);
                }
                else // Specialist
                {
                    specialistCount++;
                    lobbyPlayer.ServerSetPlayerRole(PlayerRole.Specialist);
                    lobbyPlayer.ServerSetWorkStation(randomWorkstation, PlayerRole.Specialist, true);
                }
            }

            // Apply Win Condition
            manager.RpcResetTerminationTimer(manager.terminationMaxTime);
            manager.SetWinCondition(specialistCount, slackerCount);

            // Spawn Desk Items for Unassigned Desks
            if (ConfigHandler.Gameplay.SpawnUnassignedDeskItems.Value)
                foreach (var station in GameObject.FindObjectsByType<WorkStationController>(FindObjectsSortMode.None))
                {
                    NetworkIdentity owner = station.NetworkownerLobbyPlayer;
                    if ((owner != null)
                        && !owner.WasCollected)
                        continue;

                    station.SpawnDeskItems(PlayerRole.None);
                }

            // Prevent Original
            return false;
        }
    }
}
