using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_Fax
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fax), nameof(Fax.InvokeUserCode_CmdFax__NetworkIdentity))]
        private static bool InvokeUserCode_CmdFax__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Fax
            Fax fax = __0.TryCast<Fax>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, fax.transform.position))
                return false;

            // Run Game Command
            fax.UserCode_CmdFax__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
