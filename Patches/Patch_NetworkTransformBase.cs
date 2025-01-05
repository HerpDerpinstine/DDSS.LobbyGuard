using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkTransformBase
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkTransformBase), nameof(NetworkTransformBase.InvokeUserCode_CmdTeleport__Vector3))]
        private static bool InvokeUserCode_CmdTeleport__Vector3_Prefix(NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkTransformBase), nameof(NetworkTransformBase.InvokeUserCode_CmdTeleport__Vector3__Quaternion))]
        private static bool InvokeUserCode_CmdTeleport__Vector3__Quaternion_Prefix(NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Prevent Original
            return false;
        }
    }
}
