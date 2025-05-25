using DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Internal;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.TrashBin;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_TrashBin
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.RpcEnableFire))]
        private static void RpcEnableFire_Postfix(TrashBin __instance, bool __1)
        {
            // TrashBin Security
            __instance.isOnFire = __1;
            if (!__1)
                TrashBinSecurity.OnEnableFireEnd(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(TrashBin __instance, NetworkIdentity __0, bool __1)
        {
            // TrashBin Security
            __instance.isOnFire = __1;
            TrashBinSecurity.OnEnableFireBegin(__0, __instance, __1);

            // Prevent Original
            return false;
        }
    }
}
