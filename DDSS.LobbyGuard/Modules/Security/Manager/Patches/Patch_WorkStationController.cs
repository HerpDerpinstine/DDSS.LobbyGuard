using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Security.Manager.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_WorkStationController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();
            if ((station == null)
                || station.WasCollected
                || !station.NetworkisEmpty)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, station.transform.position, InteractionSecurity.MAX_DISTANCE_EXTENDED))
                return false;

            // Get Target
            __1.SafeReadNetworkIdentity();
            NetworkIdentity targetIdentity = __1.SafeReadNetworkIdentity();
            if ((targetIdentity == null)
                || targetIdentity.WasCollected)
                return false;

            LobbyPlayer target = targetIdentity.GetComponent<LobbyPlayer>();
            if ((target == null)
                || target.WasCollected
                || target.IsGhost()
                || (target.IsJanitor() && !Extras.MoreRoleSettings.ModuleConfig.Instance.AllowJanitorsToKeepWorkStation.Value))
                return false;

            station.UserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient(sender, target, __2);

            // Prevent Original
            return false;
        }
    }
}
