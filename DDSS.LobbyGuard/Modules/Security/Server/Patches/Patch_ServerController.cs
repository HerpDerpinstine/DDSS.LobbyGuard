using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.ServerRack;

namespace DDSS_LobbyGuard.Modules.Security.Server.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ServerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.InvokeUserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get ServerController
            ServerController server = __0.TryCast<ServerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, server.transform.position))
                return false;

            // Get Values
            __1.SafeReadNetworkIdentity();
            bool enabled = __1.SafeReadBool();

            if (enabled == ServerController.connectionsEnabled)
                return false;

            // Check for Disable
            if (!enabled)
            {
                PlayerController controller = sender.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected)
                    return false;

                LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
                if ((lobbyPlayer == null)
                    || lobbyPlayer.WasCollected
                    || !lobbyPlayer.IsSlacker())
                    return false;
            }

            // Run Game Command
            server.UserCode_CmdSetConnectionEnabled__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, enabled, __2);

            // Prevent Original
            return false;
        }
    }
}
