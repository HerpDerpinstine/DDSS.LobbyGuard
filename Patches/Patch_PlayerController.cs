using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_PlayerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdSpank__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSpank__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerController
            PlayerController player = __0.TryCast<PlayerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position, 
                player.transform.position, 
                InteractionSecurity.MAX_SPANK_DISTANCE))
                return false;

            // Run Game Command
            player.UserCode_CmdSpank__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
