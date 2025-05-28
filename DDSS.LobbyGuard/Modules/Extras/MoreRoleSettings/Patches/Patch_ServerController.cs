using DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Internal;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.ServerRack;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ServerController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.RpcSetConnectionEnabled))]
        private static void RpcEnableFire_Postfix(ServerController __instance, bool __1)
        {
            ServerController.connectionsEnabled = __1;
            if (!__1)
                ServerSecurity.OnOutageEnd(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.UserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool UserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(ServerController __instance, NetworkIdentity __0, bool __1)
        {
            ServerSecurity.OnOutageBegin(__0, __instance, __1);

            // Prevent Original
            return false;
        }
    }
}
