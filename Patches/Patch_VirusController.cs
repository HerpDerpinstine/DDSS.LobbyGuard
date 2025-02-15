﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
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
            if (!ConfigHandler.Gameplay.WorkStationVirusTurnsOffFirewall.Value 
                || !__0)
                return;

            __instance.UserCode_CmdSetFireWall__NetworkIdentity__Boolean__NetworkConnectionToClient(
                __instance.netIdentity,
                false,
                __instance.netIdentity.connectionToClient);
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

            if (NetworkServer.activeHost)
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

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.InvokeUserCode_CmdSetFireWall__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetFireWall__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
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
            __1.ReadNetworkIdentity();
            bool state = __1.SafeReadBool();
            if (controller.NetworkisFirewallActive == state)
                return false;

            // Run Game Command
            controller.UserCode_CmdSetFireWall__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, state, __2);

            // Prevent Original
            return false;
        }
    }
}
