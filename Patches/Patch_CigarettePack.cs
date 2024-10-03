using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Smoking;

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

            // Get Cigarette from Prefab
            Cigarette prefabCig = cigarettePack.cigarettePrefab.GetComponentInChildren<Cigarette>();
            if (prefabCig == null)
                return false;

            // Get Cigarette Interactable Name
            string interactableName = prefabCig.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                InteractionSecurity.MAX_CIGS_PER_PACK))
                return false;

            // Run Game Command
            cigarettePack.UserCode_CmdSmokeCigarette__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
