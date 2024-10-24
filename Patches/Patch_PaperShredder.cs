using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.PaperShredder;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PaperShredder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_CmdStartInteraction__NetworkIdentity))]
        private static bool InvokeUserCode_CmdStartInteraction__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PaperShredder ~ CURSE YOUR TURTLES!!!
            PaperShredder shredder = __0.TryCast<PaperShredder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, shredder.transform.position))
                return false;

            // Run Game Command
            shredder.UserCode_CmdStartInteraction__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
