using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.ServerRack;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Server.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ServerController
    {
        private static float outageTime;
        private static float outageTimeLimit;

        private static bool ShouldWaitForStart(ServerController server)
        {
            if (!server.mainServer
                || !server.isServer
                || (TutorialManager.instance != null))
                return true;

            if ((GameManager.instance.NetworktargetGameState == (int)GameStates.WaitingForPlayerConnections)
                    || (GameManager.instance.NetworktargetGameState == (int)GameStates.StartGame)
                    || (GameManager.instance.NetworktargetGameState == (int)GameStates.GameFinished))
                return true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.Start))]
        private static bool Start_Prefix(ServerController __instance)
        {
            ServerController.connectionsEnabled = true;
            __instance.previousConnectionState = true;

            outageTime = 0f;
            outageTimeLimit = Random.Range(ModuleConfig.Instance.ServerOutageRandomMinimum.Value, ModuleConfig.Instance.ServerOutageRandomMaximum.Value);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.Update))]
        private static bool Update_Prefix(ServerController __instance)
        {
            if (NetworkServer.activeHost
                && ServerController.connectionsEnabled
                && !ShouldWaitForStart(__instance))
            {
                outageTime += Time.deltaTime;
                if (outageTime > outageTimeLimit)
                {
                    outageTime = 0f;
                    outageTimeLimit = Random.Range(ModuleConfig.Instance.ServerOutageRandomMinimum.Value, ModuleConfig.Instance.ServerOutageRandomMaximum.Value);
                    ServerController.connectionsEnabled = false;
                    __instance.RpcSetConnectionEnabled(null, false);
                }
            }

            if (ServerController.connectionsEnabled != __instance.previousConnectionState)
            {
                __instance.previousConnectionState = ServerController.connectionsEnabled;

                if (ServerController.connectionsEnabled)
                    __instance.audioSource.clip = __instance.normalSound;
                else
                    __instance.audioSource.clip = __instance.alarmSound;
                __instance.audioSource.Play();
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.InvokeUserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get ServerController
            ServerController server = __0.TryCast<ServerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, server))
                return false;

            // Get Values
            __1.SafeReadNetworkIdentity();
            bool enabled = __1.SafeReadBool();

            if (enabled == ServerController.connectionsEnabled)
                return false;

            // Check for Disable
            if (!enabled)
            {
                PlayerController controller = sender.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected)
                    return false;

                LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
                if ((lobbyPlayer == null)
                    || lobbyPlayer.WasCollected
                    || !lobbyPlayer.IsSlacker())
                    return false;
            }

            // Run Game Command
            server.UserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, enabled, __2);

            // Prevent Original
            return false;
        }
    }
}
