﻿using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FilingCabinetController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FilingCabinetController), nameof(FilingCabinetController.InvokeUserCode_CmdSetUnorganized__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetUnorganized__NetworkConnectionToClient_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
