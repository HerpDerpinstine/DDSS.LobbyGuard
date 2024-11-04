using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
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

            // Check if Drawer is Open and Unorganized
            if (!drawer.NetworkisOpen
                || drawer.NetworkisOrganized)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, drawer.transform.position))
                return false;

            // Run Game Command
            drawer.UserCode_CmdOrganize__NetworkIdentity(sender);
            drawer.filingCabinetController.RpcCalculateIsOrganized(sender);

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
            if (drawer.NetworkisOpen == requestedState)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, drawer.transform.position))
                return false;

            // Apply State
            drawer.NetworkisOpen = requestedState;
            drawer.RpcSetDrawerState(sender, requestedState);
            if (sender.isLocalPlayer)
                drawer.filingCabinetController.RpcCalculateIsOrganized(sender);

            // Reset Organization for Entire Cabinet
            if (requestedState)
            {
                // Check if Every Drawer has been Organized
                bool isAllOrganized = false;
                foreach (DrawerController drawerController in drawer.filingCabinetController.drawers)
                {
                    if (!drawerController.NetworkisOrganized)
                    {
                        isAllOrganized = false;
                        break;
                    }
                    else
                        isAllOrganized = true;
                }

                // Unorganize Drawers
                if (isAllOrganized)
                {
                    drawer.filingCabinetController.UserCode_CmdSetUnorganized();
                    if (sender.isLocalPlayer)
                        drawer.filingCabinetController.RpcCalculateIsOrganized(sender);
                }
            }

            // Prevent Original
            return false;
        }
    }
}
