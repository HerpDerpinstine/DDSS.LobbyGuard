using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.WC.Toilet;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Toilet
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Toilet), nameof(Toilet.InvokeUserCode_CmdShit__NetworkIdentity))]
        private static bool InvokeUserCode_CmdShit__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Toilet
            Toilet toilet = __0.TryCast<Toilet>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, toilet.transform.position))
                return false;

            // Run Game Command
            toilet.UserCode_CmdShit__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
