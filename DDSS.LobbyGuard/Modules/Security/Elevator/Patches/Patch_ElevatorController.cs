using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Elevator;

namespace DDSS_LobbyGuard.Modules.Security.Server.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ElevatorController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ElevatorController), nameof(ElevatorController.InvokeUserCode_CmdSetTargetLevel__NetworkIdentity__Int32))]
        private static bool InvokeUserCode_CmdSetTargetLevel__NetworkIdentity__Int32_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get ElevatorController
            ElevatorController elevator = __0.TryCast<ElevatorController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !elevator.enabled
                || !InteractionSecurity.IsWithinRange(sender.transform.position, elevator.transform.position))
                return false;

            __1.SafeReadNetworkIdentity();
            int targetLevel = __1.SafeReadInt();
            if (targetLevel < 0)
                targetLevel = 0;
            if (targetLevel > 1)
                targetLevel = 1;

            // Run Game Command
            elevator.UserCode_CmdSetTargetLevel__NetworkIdentity__Int32(sender, targetLevel);

            // Prevent Original
            return false;
        }
    }
}