using HarmonyLib;
using Il2Cpp;
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
        private static bool UserCode_CmdSetDoorState__Int32_Prefix(
            DoorController __instance,
            int __0)
        {
            __0 = Mathf.Clamp(__0, -1, 1);

            // Check for Lock
            if (__0 == 0
                || __instance.NetworkisLocked)
                return false;

            // Check if already Open
            if (__instance.Networkstate != 0)
                return false;

            // Apply State
            ModuleMain.ApplyState(__instance, __0);

            // Prevent Original
            return false;
        }
    }
}
