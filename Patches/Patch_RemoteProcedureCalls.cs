using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror.RemoteCalls;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_RemoteProcedureCalls
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RemoteProcedureCalls), nameof(RemoteProcedureCalls.RegisterRpc))]
        private static void RegisterRpc_Prefix(string __1)
        {
            RPCHelper.OnRegistered(__1);
        }
    }
}
