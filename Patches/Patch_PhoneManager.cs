using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_PhoneManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.CmdCall))]
        private static bool CmdCall_Prefix(string __0, string __1)
        {
            // Validate
            if (__0 == __1)
                return false;

            // Run Original
            return true;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneManager), nameof(PhoneManager.UserCode_CmdCall__String__String))]
        private static bool UserCode_CmdCall__String__String_Prefix(string __0, string __1)
        {
            // Validate
            if (__0 == __1)
                return false;

            // Run Original
            return true;
        }
    }
}
