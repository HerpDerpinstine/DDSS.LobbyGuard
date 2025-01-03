﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_VirusController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.ServerSetVirus))]
        private static void ServerSetVirus_Postfix(VirusController __instance, bool __0)
        {
            if (!__0)
                return;
            if (ConfigHandler.Gameplay.WorkStationVirusTurnsOffFirewall.Value)
                __instance.UserCode_CmdSetFireWall__Boolean(false);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.Start))]
        private static bool Start_Prefix(VirusController __instance)
        {
            __instance.computerController = __instance.GetComponent<ComputerController>();
            __instance.virusObj.SetActive(false);
            __instance.time = 1f;
            __instance.virusScreen.color = Color.blue;
            __instance.isFirewallActive = true;
            __instance.isVirusActive = false;
            __instance.virusInfectionTime = 0f;
            __instance.virusInfectionTimeLimit = 120f;

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

            if ((!ConfigHandler.Gameplay.WorkStationVirusResetsRandomVirusTimer.Value || !__instance.isVirusActive)
                && NetworkServer.activeHost
                && (GameManager.instance.currentGameState > (int)GameStates.WaitingForPlayerConnections)
                && (__instance.computerController.user != null)
                && !__instance.computerController.user.WasCollected)
            {
                __instance.virusInfectionTime += Time.deltaTime;

                if (__instance.virusInfectionTime >= __instance.virusInfectionTimeLimit)
                {
                    int userMin = ConfigHandler.Gameplay.RandomWorkStationVirusDelayMin.Value;
                    int userMax = ConfigHandler.Gameplay.RandomWorkStationVirusDelayMax.Value;
                    if (userMax < userMin)
                    {
                        int origMin = userMin;
                        userMin = userMax;
                        userMax = origMin;
                    }

                    __instance.virusInfectionTime = 0f;
                    __instance.virusInfectionTimeLimit = Random.Range(userMin, userMax);
                    __instance.PerformPotentialVirusActivity();
                }
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.PerformPotentialVirusActivity))]
        private static bool PerformPotentialVirusActivity_Prefix(VirusController __instance)
        {
            // Validate Server
            if (!NetworkServer.activeHost
                || (GameManager.instance.currentGameState <= (int)GameStates.WaitingForPlayerConnections)
                || __instance.isVirusActive)
                return false;

            // Validate Workstation
            if ((__instance.computerController == null)
                || (__instance.computerController.user == null))
                return false;

            // Validate Role
            if (!__instance.NetworkisFirewallActive
                || InteractionSecurity.IsSlacker(__instance.computerController.user))
            {
                // Get Game Rule
                float probability = GameManager.instance.virusProbability;

                // RandomGen
                if (Random.Range(0f, 100f) < (probability * 100f))
                    __instance.ServerSetVirus(true);
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.InvokeUserCode_CmdSetFireWall__Boolean))]
        private static bool InvokeUserCode_CmdSetFireWall__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get VirusController
            VirusController controller = __0.TryCast<VirusController>();
            if (controller == null)
                return false;

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller.computerController, sender))
                return false;

            // Get Value
            bool state = __1.SafeReadBool();
            if (controller.NetworkisFirewallActive == state)
                return false;

            // Run Game Command
            controller.UserCode_CmdSetFireWall__Boolean(state);

            // Prevent Original
            return false;
        }
    }
}
