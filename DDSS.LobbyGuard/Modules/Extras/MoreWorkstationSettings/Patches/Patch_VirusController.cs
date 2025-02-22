using HarmonyLib;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
using UnityEngine;
using DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Internal;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_VirusController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.PerformPotentialVirusActivity))]
        private static bool PerformPotentialVirusActivity_Prefix(VirusController __instance)
        {
            // Prevent Original
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.ServerSetVirus))]
        private static void ServerSetVirus_Postfix(VirusController __instance, bool __0)
        {
            if (!__0)
                return;
            else if (ModuleConfig.Instance.WorkStationVirusTurnsOffFirewall.Value)
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
            __instance.virusInfectionTimeLimit = 120f;

            VirusHandler.OnStart(__instance);

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

            // Prevent Original
            return false;
        }
    }
}
