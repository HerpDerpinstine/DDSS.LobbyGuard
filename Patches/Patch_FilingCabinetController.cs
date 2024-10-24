using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FilingCabinetController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FilingCabinetController), nameof(FilingCabinetController.InvokeUserCode_CmdSetUnorganized))]
        private static bool InvokeUserCode_CmdSetUnorganized_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
