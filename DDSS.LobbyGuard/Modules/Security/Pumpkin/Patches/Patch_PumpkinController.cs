using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Pumpkin.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PumpkinController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PumpkinController), nameof(PumpkinController.InvokeUserCode_CmdSetLit__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetLit__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
             NetworkBehaviour __0,
             NetworkReader __1,
             NetworkConnectionToClient __2)
        {
            // Get PumpkinController
            PumpkinController pumpkin = __0.TryCast<PumpkinController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, pumpkin.transform.position))
                return false;

            __1.SafeReadNetworkIdentity();
            bool shouldLight = __1.SafeReadBool();

            // Run Game Command
            pumpkin.UserCode_CmdSetLit__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, shouldLight, sender.connectionToClient);

            // Prevent Original
            return false;
        }
    }
}