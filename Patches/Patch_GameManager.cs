using HarmonyLib;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using System;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_GameManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdFirePlayer__NetworkIdentity__Boolean__Boolean))]
        private static bool InvokeUserCode_CmdFirePlayer__NetworkIdentity__Boolean__Boolean_Prefix(NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Validate Manager Role
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdSetAssistant__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSetAssistant__NetworkIdentity_Prefix(NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Validate Manager Role
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.InvokeUserCode_CmdSetState__Int32))]
        private static bool InvokeUserCode_CmdSetState__Int32_Prefix(NetworkBehaviour __0,
            NetworkReader __1, 
            NetworkConnectionToClient __2)
        {
            // Get Requested State
            GameManager manager = __0.TryCast<GameManager>();

            // Get Requested State
            GameStates requestedState = (GameStates)__1.ReadInt();

            // Get Current State
            GameStates state = (GameStates)manager.NetworktargetGameState;

            // Parse States
            switch (state)
            {
                // Tutorial -> Any
                case GameStates.Tutorial:
                    break;

                // WaitingForPlayerConnections -> StartGame or Tutorial
                case GameStates.WaitingForPlayerConnections:
                    // Check Requested State
                    if ((requestedState != GameStates.StartGame)
                        && (requestedState != GameStates.Tutorial))
                        return false;

                    // Check for Host
                    if (!__2.identity.isServer)
                        return false;

                    break;

                // StartGame -> InGame
                case GameStates.StartGame:
                    // Check Requested State
                    if (requestedState != GameStates.InGame)
                        return false;

                    // Check for Host
                    if (!__2.identity.isServer)
                        return false;

                    break;

                // InGame -> Meeting or GameFinished
                case GameStates.InGame:
                    // Check Requested State
                    if ((requestedState != GameStates.Meeting)
                        && (requestedState != GameStates.GameFinished))
                        return false;

                    // Check for Host
                    if (!__2.identity.isServer)
                    {
                        // Meeting was Requested
                        if (requestedState == GameStates.Meeting)
                        {
                            // Check Requesting Player Role
                            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                            if ((player == null)
                                || (player.playerRole != PlayerRole.Manager))
                                return false;
                        }

                        // Game is Done
                        if (requestedState == GameStates.GameFinished)
                            return false;
                    }

                    break;

                // Meeting -> InGame or GameFinished
                case GameStates.Meeting:
                    // Check Requested State
                    if ((requestedState != GameStates.InGame)
                        && (requestedState != GameStates.GameFinished))
                        return false;

                    // Check for Host
                    if (!__2.identity.isServer)
                    {
                        // Meeting has Ended
                        if (requestedState == GameStates.InGame)
                        {
                            // Check Requesting Player Role
                            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                            if ((player == null)
                                || (player.playerRole != PlayerRole.Manager))
                                return false;
                        }

                        // Game is Done
                        if (requestedState == GameStates.GameFinished)
                            return false;
                    }

                    break;

                // GameFinished -> Scene Unloads
                case GameStates.GameFinished:
                    return false;

                // Default Case
                default:
                    // Check for Host
                    if (!__2.identity.isServer)
                        return false;

                    break;
            }

            // Set State
            manager.UserCode_CmdSetState__Int32((int)requestedState);

            // Prevent Original
            return false;
        }
    }
}