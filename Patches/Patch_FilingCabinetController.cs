using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FilingCabinetController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FilingCabinetController), nameof(FilingCabinetController.InvokeUserCode_CmdSetUnorganized__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetUnorganized__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get FilingCabinetController
            FilingCabinetController cabinet = __0.TryCast<FilingCabinetController>();

            // Check if Drawer is Open and Unorganized
            if (!cabinet.IsCabinetOrganized())
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, cabinet.transform.position))
                return false;

            // Run Game Command
            cabinet.UserCode_CmdSetUnorganized__NetworkConnectionToClient(__2);

            // Prevent Original
            return false;
        }
    }
}
