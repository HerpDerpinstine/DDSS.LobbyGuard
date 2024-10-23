using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_DrawerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DrawerController), nameof(DrawerController.InvokeUserCode_CmdOrganize__NetworkIdentity))]
        private static bool InvokeUserCode_CmdOrganize__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get DrawerController
            DrawerController drawer = __0.TryCast<DrawerController>();

            // Check if Drawer is Open
            if (!drawer.isOpen)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, drawer.transform.position))
                return false;

            // Run Game Command
            drawer.UserCode_CmdOrganize__NetworkIdentity(sender);

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

            // Get State
            __1.ReadNetworkIdentity();
            bool requestedState = __1.ReadBool();
            if (drawer.isOpen == requestedState)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, drawer.transform.position))
                return false;

            // Apply State
            drawer.UserCode_CmdSetDrawerState__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, requestedState, __2);

            // Prevent Original
            return false;
        }
    }
}
