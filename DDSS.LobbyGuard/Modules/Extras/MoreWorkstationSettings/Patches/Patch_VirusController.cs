using HarmonyLib;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
using UnityEngine;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_VirusController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.ServerSetVirus))]
        private static void ServerSetVirus_Postfix(VirusController __instance, bool __0)
        {
            if (!ModuleConfig.Instance.WorkStationVirusTurnsOffFirewall.Value
                || !__0)
                return;

            __instance.UserCode_CmdSetFireWall__NetworkIdentity__Boolean__NetworkConnectionToClient(
                __instance.netIdentity,
                false,
                __instance.netIdentity.connectionToClient);
        }
    }
}
