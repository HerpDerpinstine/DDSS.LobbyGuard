using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppProps.ServerRack;
using System.Collections;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Game.Patches
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
            outageTimeLimit = Random.Range(120, 600);

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
                    outageTimeLimit = Random.Range(120, 600);
                    ServerController.connectionsEnabled = false;
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
    }
}
