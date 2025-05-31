using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Easel;

namespace DDSS_LobbyGuard.Modules.Security.Analyst.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_EaselController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EaselController), nameof(EaselController.InvokeUserCode_CmdShowStatistics__NetworkIdentity__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdShowStatistics__NetworkIdentity__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if ((GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return false;

            // Get EaselController
            if ((__0 == null)
                || __0.WasCollected)
                return false;
            EaselController easel = __0.TryCast<EaselController>();
            if ((easel == null)
                || easel.WasCollected)
                return false;

            // Get Sender
            if ((__2 == null)
                || __2.WasCollected)
                return false;

            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;
            if (sender.GetPlayerSubRole() != SubRole.Analyst)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, easel.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            __1.SafeReadNetworkIdentity();

            NetworkIdentity targetA = __1.SafeReadNetworkIdentity();
            if ((targetA == null)
                || targetA.WasCollected)
                return false;
            NetworkIdentity targetB = __1.SafeReadNetworkIdentity();
            if ((targetB == null)
                || targetB.WasCollected)
                return false;

            easel.UserCode_CmdShowStatistics__NetworkIdentity__NetworkIdentity__NetworkIdentity__NetworkConnectionToClient(sender, targetA, targetB, __2);

            // Prevent Original
            return false;
        }
    }
}
