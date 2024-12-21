using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FilingCabinetController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FilingCabinetController), nameof(FilingCabinetController.InvokeUserCode_CmdSetUnorganized))]
        private static bool InvokeUserCode_CmdSetUnorganized_Prefix(NetworkConnectionToClient __2)
        {
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }
    }
}
