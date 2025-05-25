using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.CameraProp;

namespace DDSS_LobbyGuard.Modules.Security.Assistant.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CameraPropController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraPropController), nameof(CameraPropController.InvokeUserCode_CmdCapture__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCapture__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get CameraPropController
            CameraPropController camera = __0.TryCast<CameraPropController>();
            if ((camera == null)
                || camera.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender.GetPlayerSubRole() != SubRole.Assistant)
                && (sender.GetPlayerRole() != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, camera.transform.position))
                return false;

            // Validate Placement
            Il2CppProps.Scripts.Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get CameraPropController
            camera = collectible.TryCast<CameraPropController>();
            if (camera == null)
                return false;

            camera.UserCode_CmdCapture__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CameraPropController), nameof(CameraPropController.InvokeUserCode_CmdUseOverride__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdUseOverride__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get CameraPropController
            CameraPropController camera = __0.TryCast<CameraPropController>();
            if ((camera == null)
                || camera.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender.GetPlayerSubRole() != SubRole.Assistant)
                && (sender.GetPlayerRole() != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, camera.transform.position))
                return false;

            camera.UserCode_CmdUseOverride__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
