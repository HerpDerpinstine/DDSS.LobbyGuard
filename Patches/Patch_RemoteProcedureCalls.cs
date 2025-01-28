using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror.RemoteCalls;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_RemoteProcedureCalls
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RemoteProcedureCalls), nameof(RemoteProcedureCalls.RegisterDelegate))]
        private static void RegisterDelegate_Prefix(string __1, RemoteCallType __2)
        {
            if (__2 == RemoteCallType.ClientRpc)
                RPCHelper.OnRegister(__1);
        }
    }
}
