using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Sink.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_SinkController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SinkController), nameof(SinkController.InvokeUserCode_CmdStartWashingHands__NetworkIdentity))]
        private static bool InvokeUserCode_CmdStartWashingHands__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get SinkController
            SinkController sink = __0.TryCast<SinkController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, sink))
                return false;

            // Run Game Command
            sink.UserCode_CmdStartWashingHands__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
