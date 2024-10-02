using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_CigarettePack
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CigarettePack), nameof(CigarettePack.InvokeUserCode_CmdSmokeCigarette__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSmokeCigarette__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CigarettePack
            CigarettePack cigarettePack = __0.TryCast<CigarettePack>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cigarettePack.transform.position))
                return false;

            // Run Game Command
            cigarettePack.UserCode_CmdSmokeCigarette__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
