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
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.Start))]
        private static bool Start_Prefix(VirusController __instance)
        {
            __instance.computerController = __instance.GetComponent<ComputerController>();
            __instance.virusObj.SetActive(false);
            __instance.time = 1f;
            __instance.virusScreen.color = Color.blue;

            if (!NetworkServer.activeHost)
                return false;
            
            __instance.NetworkisFirewallActive = true;
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

                if (__instance.computerController.user != null)
                {
                    if (!NetworkServer.activeHost
                        || (GameManager.instance.currentGameState <= (int)GameStates.WaitingForPlayerConnections))
                        return false;

                    __instance.virusInfectionTime += Time.deltaTime;

                    if (__instance.virusInfectionTime >= __instance.virusInfectionTimeLimit)
                    {
                        __instance.PerformPotentialVirusActivity();
                        __instance.virusInfectionTime = 0f;
                        __instance.virusInfectionTimeLimit = Random.Range(30, 45);
                    }
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
                || (GameManager.instance.currentGameState <= (int)GameStates.WaitingForPlayerConnections))
                return false;

            // Validate Workstation
            if ((__instance.computerController == null)
                || (__instance.computerController.user == null))
                return false;

            // Validate Role
            if (InteractionSecurity.IsSlacker(__instance.computerController.user)
                || !__instance.NetworkisFirewallActive)
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
