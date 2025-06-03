using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Shelf.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_DrawerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DrawerController), nameof(DrawerController.InvokeUserCode_CmdOrganize__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdOrganize__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get DrawerController
            DrawerController drawer = __0.TryCast<DrawerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, drawer))
                return false;

            // Run Game Command
            drawer.UserCode_CmdOrganize__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DrawerController), nameof(DrawerController.InvokeUserCode_CmdSetDrawerState__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetDrawerState__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get DrawerController
            DrawerController drawer = __0.TryCast<DrawerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, drawer))
                return false;

            // Get State
            __1.SafeReadNetworkIdentity();
            bool requestedState = __1.SafeReadBool();

            // Run Game Command
            drawer.UserCode_CmdSetDrawerState__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, requestedState, __2);

            // Prevent Original
            return false;
        }
    }
}