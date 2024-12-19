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
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_CmdStartInteraction__NetworkIdentity))]
        private static bool InvokeUserCode_CmdStartInteraction__NetworkIdentity_Prefix(
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
            shredder.UserCode_CmdStartInteraction__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperShredder), nameof(PaperShredder.InvokeUserCode_TimerCallback__Single))]
        private static bool InvokeUserCode_TimerCallback__Single_Prefix(
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
            float timer = __1.SafeReadFloat();

            // Run Game Command
            shredder.UserCode_TimerCallback__Single(timer);

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
