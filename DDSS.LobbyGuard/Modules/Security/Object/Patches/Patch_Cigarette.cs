using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;
using Il2CppProps.Smoking;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Cigarette
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Cigarette), nameof(Cigarette.InvokeUserCode_CmdSmokeCigarette__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSmokeCigarette__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Cigarette
            Cigarette cigarette = __0.TryCast<Cigarette>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, cigarette))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<Cigarette>()))
                return false;

            // Get Cigarette
            cigarette = collectible.TryCast<Cigarette>();
            if (cigarette == null)
                return false;

            // Run Game Command
            cigarette.UserCode_CmdSmokeCigarette__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}