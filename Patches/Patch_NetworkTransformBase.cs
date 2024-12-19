using HarmonyLib;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkTransformBase
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkTransformBase), nameof(NetworkTransformBase.InvokeUserCode_CmdTeleport__Vector3))]
        private static bool InvokeUserCode_CmdTeleport__Vector3_Prefix(NetworkConnectionToClient __2)
        {
            // Check for Server
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkTransformBase), nameof(NetworkTransformBase.InvokeUserCode_CmdTeleport__Vector3__Quaternion))]
        private static bool InvokeUserCode_CmdTeleport__Vector3__Quaternion_Prefix(NetworkConnectionToClient __2)
        {
            // Check for Server
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }
    }
}
