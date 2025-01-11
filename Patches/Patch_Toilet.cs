using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.WC.Toilet;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Toilet
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Toilet), nameof(Toilet.InvokeUserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Toilet
            Toilet toilet = __0.TryCast<Toilet>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Seat
            if (toilet.NetworkusingPlayer != sender)
                return false;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, toilet.transform.position))
                return false;

            // Run Game Command
            toilet.UserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient(sender, sender.connectionToClient);

            // Prevent Original
            return false;
        }
    }
}
