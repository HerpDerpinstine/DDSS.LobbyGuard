using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Tasks;

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

            // Check if Drawer is Open and Unorganized
            if (!drawer.NetworkisOpen
                || drawer.NetworkisOrganized)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, drawer))
                return false;

            // Run Game Command
            drawer.UserCode_CmdOrganize__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            drawer.filingCabinetController.RpcCalculateIsOrganized(sender);

            // Manually Trigger Task Locally
            if (sender.isLocalPlayer)
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
                if (isAllOrganized)
                    TaskHook.TriggerTaskHookCommandStatic(new TaskHook(null, "Filing Cabinet", null, "Organized", drawer.filingCabinetController.roomTrigger.currentRoom.roomName, null));
            }

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
            if (drawer.NetworkisOpen == requestedState)
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
                    drawer.filingCabinetController.UserCode_CmdSetUnorganized__NetworkConnectionToClient(__2);
                    if (sender.isLocalPlayer)
                    {
                        drawer.filingCabinetController.RpcCalculateIsOrganized(sender);
                        TaskHook.TriggerTaskHookCommandStatic(new TaskHook(null, "Filing Cabinet", null, "Organized", drawer.filingCabinetController.roomTrigger.currentRoom.roomName, null));
                    }
                }
            }

            // Prevent Original
            return false;
        }
    }
}