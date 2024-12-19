using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_CCTVController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CCTVController), nameof(CCTVController.InvokeUserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Get CCTVController
            CCTVController camera = __0.TryCast<CCTVController>();
            if ((camera == null)
                || camera.WasCollected
                || !camera.isActivated)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, camera.transform.position))
                return false;

            // Run Game Command
            camera.UserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
