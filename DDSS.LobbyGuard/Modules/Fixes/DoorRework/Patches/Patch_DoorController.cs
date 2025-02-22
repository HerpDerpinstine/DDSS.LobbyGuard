using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Fixes.DoorRework.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_DoorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.Start))]
        private static void Start_Prefix()
        {
            // Start Door
            ModuleMain.DoorStart();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DoorController), nameof(DoorController.UserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient))]
        private static void UserCode_CmdSetDoorState__Int32__PlayerController__NetworkConnectionToClient_Prefix(DoorController __instance)
        {
            ModuleMain.FixColliderSize(__instance.playerDetectionVolumeForward);
            ModuleMain.FixColliderSize(__instance.playerDetectionVolumeBackward);
        }
    }
}
