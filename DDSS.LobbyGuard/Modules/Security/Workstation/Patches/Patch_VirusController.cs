using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using UnityEngine;
using Il2CppComputer.Scripts.System;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using DDSS_LobbyGuard.Modules.Security.Workstation.Internal;

namespace DDSS_LobbyGuard.Modules.Security.Workstation.Patches
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
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.ServerSetVirus))]
        private static void ServerSetVirus_Prefix(VirusController __instance, bool __0)
        {
            __instance.virusInfectionTime = 0f;
            __instance.virusInfectionTimeLimit = Random.Range(ModuleConfig.Instance.WorkstationVirusRandomMinimum.Value,
                ModuleConfig.Instance.WorkstationVirusRandomMaximum.Value);
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
            __instance.virusInfectionTimeLimit = Random.Range(ModuleConfig.Instance.WorkstationVirusRandomMinimum.Value,
                ModuleConfig.Instance.WorkstationVirusRandomMaximum.Value);

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
                    __instance.virusInfectionTimeLimit = Random.Range(ModuleConfig.Instance.WorkstationVirusRandomMinimum.Value,
                        ModuleConfig.Instance.WorkstationVirusRandomMaximum.Value);
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
