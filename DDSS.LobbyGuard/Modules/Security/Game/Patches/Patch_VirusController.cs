using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using UnityEngine;
using Il2CppComputer.Scripts.System;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;

namespace DDSS_LobbyGuard.Modules.Security.Game.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_VirusController
    {
        private static bool ShouldWaitForStart()
        {
            if ((GameManager.instance.NetworktargetGameState == (int)GameStates.WaitingForPlayerConnections)
                    || (GameManager.instance.NetworktargetGameState == (int)GameStates.StartGame)
                    || (GameManager.instance.NetworktargetGameState == (int)GameStates.GameFinished))
                return true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.Start))]
        private static bool Start_Prefix(VirusController __instance)
        {
            __instance.computerController = __instance.GetComponent<ComputerController>();
            __instance.virusObj.SetActive(false);
            __instance.time = 1f;
            __instance.virusScreen.color = Color.blue;
            __instance.NetworkisFirewallActive = true;
            __instance.isFirewallActive = true;
            __instance.isVirusActive = false;
            __instance.virusInfectionTime = 0f;
            __instance.virusInfectionTimeLimit = 120f + Random.Range(30, 45);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.Update))]
        private static bool Update_Prefix(VirusController __instance)
        {
            __instance.virusObj.SetActive(__instance.isVirusActive);

            if (__instance.isVirusActive)
            {
                __instance.time += Time.deltaTime;

                if (__instance.time >= 1f)
                    __instance.virusScreen.color = Color.blue;
                else
                    __instance.virusScreen.color = Color.black;

                if (__instance.time >= 2f)
                    __instance.time = 0f;
            }
            else
            {
                __instance.time = 1f;
                __instance.virusScreen.color = Color.blue;
            }

            if (NetworkServer.activeHost 
                && !__instance.isVirusActive
                && !ShouldWaitForStart())
            {
                __instance.virusInfectionTime += Time.deltaTime;
                if (__instance.virusInfectionTime > __instance.virusInfectionTimeLimit)
                {
                    __instance.virusInfectionTime = 0f;
                    __instance.virusInfectionTimeLimit = Random.Range(30, 45);

                    if (__instance.computerController.user != null)
                        __instance.PerformPotentialVirusActivity();
                }
            }

            // Prevent Original
            return false;
        }
    }
}
