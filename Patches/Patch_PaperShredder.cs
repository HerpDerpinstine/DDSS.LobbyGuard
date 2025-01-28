using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.PaperShredder;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PaperShredder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_CmdStartInteraction__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStartInteraction__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PaperShredder ~ CURSE YOUR TURTLES!!!
            PaperShredder shredder = __0.TryCast<PaperShredder>();
            if ((shredder == null)
                || shredder.WasCollected
                || shredder.collectibles.Count <= 0)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, shredder.transform.position))
                return false;

            // Run Game Command
            shredder.UserCode_CmdStartInteraction__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient_Prefix(
           NetworkBehaviour __0,
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Get PaperShredder ~ CURSE YOUR TURTLES!!!
            PaperShredder shredder = __0.TryCast<PaperShredder>();
            if ((shredder == null)
                || shredder.WasCollected
                || shredder.collectibles.Count <= 0)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, shredder.transform.position))
                return false;

            // Read Value
            __1.SafeReadNetworkIdentity();
            float timer = __1.SafeReadFloat();
            bool initial = __1.SafeReadBool();

            // Run Game Command
            shredder.UserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient(sender, timer, initial, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_ShredDocument__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_ShredDocument__NetworkIdentity__NetworkConnectionToClient_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Get PaperShredder ~ CURSE YOUR TURTLES!!!
            PaperShredder shredder = __0.TryCast<PaperShredder>();
            if ((shredder == null)
                || shredder.WasCollected
                || shredder.collectibles.Count <= 0)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, shredder.transform.position))
                return false;

            // Run Game Command
            shredder.UserCode_ShredDocument__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
