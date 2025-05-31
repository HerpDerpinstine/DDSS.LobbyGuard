using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Assistant.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_TVController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TVController), nameof(TVController.InvokeUserCode_CmdShowPhotos__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdShowPhotos__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get TVController
            TVController TV = __0.TryCast<TVController>();
            if ((TV == null)
                || TV.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.Assistant)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, TV.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            TV.UserCode_CmdShowPhotos__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TVController), nameof(TVController.InvokeUserCode_CmdIncrementPhotoIndex__NetworkIdentity__Int32__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdIncrementPhotoIndex__NetworkIdentity__Int32__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get TVController
            TVController TV = __0.TryCast<TVController>();
            if ((TV == null)
                || TV.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.Assistant)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, TV.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            __1.SafeReadNetworkIdentity();
            int inc = __1.SafeReadInt();
            if (inc > 1)
                inc = 1;
            if (inc < 1)
                inc = -1;

            TV.UserCode_CmdIncrementPhotoIndex__NetworkIdentity__Int32__NetworkConnectionToClient(sender, inc, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TVController), nameof(TVController.InvokeUserCode_CmdEndSlideshow__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEndSlideshow__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get TVController
            TVController TV = __0.TryCast<TVController>();
            if ((TV == null)
                || TV.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.Assistant)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, TV.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            TV.UserCode_CmdEndSlideshow__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TVController), nameof(TVController.InvokeUserCode_CmdStartTransfer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStartTransfer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get TVController
            TVController TV = __0.TryCast<TVController>();
            if ((TV == null)
                || TV.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerSubRole() != SubRole.Assistant)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, TV.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            TV.UserCode_CmdStartTransfer__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
