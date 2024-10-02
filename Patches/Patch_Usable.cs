using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_Usable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdUse__NetworkIdentity))]
        private static bool InvokeUserCode_CmdUse__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Usable
            Usable usable = __0.TryCast<Usable>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Run Game Command
            usable.UserCode_CmdUse__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdUseNoTypeVerification__NetworkIdentity))]
        private static bool InvokeUserCode_CmdUseNoTypeVerification__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Usable
            Usable usable = __0.TryCast<Usable>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Run Game Command
            usable.UserCode_CmdUseNoTypeVerification__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdStopUse__NetworkIdentity))]
        private static bool InvokeUserCode_CmdStopUse__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Usable
            Usable usable = __0.TryCast<Usable>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Get Player Controller
            PlayerController player = sender.GetComponent<PlayerController>();

            // Run Game Command
            usable.UserCode_CmdStopUse__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
