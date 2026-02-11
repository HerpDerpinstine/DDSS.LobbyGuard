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
    }
}
