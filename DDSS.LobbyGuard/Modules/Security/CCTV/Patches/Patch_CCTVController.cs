using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Security.CCTV.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
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
                || camera.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = sender.GetLobbyPlayer();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, camera.transform.position, InteractionSecurity.MAX_DISTANCE_CCTV))
                return false;

            // Run Game Command
            camera.UserCode_CmdUpdateFirmware__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}