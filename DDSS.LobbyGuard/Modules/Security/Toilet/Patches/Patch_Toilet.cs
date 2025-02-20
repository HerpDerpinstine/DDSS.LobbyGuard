using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Toilet.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Toilet
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.WC.Toilet.Toilet), nameof(Il2CppProps.WC.Toilet.Toilet.InvokeUserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Toilet
            Il2CppProps.WC.Toilet.Toilet toilet = __0.TryCast<Il2CppProps.WC.Toilet.Toilet>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Role
            if (sender.IsGhost())
                return false;

            // Validate Seat
            if (toilet.usingPlayerController != sender)
                return false;

            // Run Game Command
            toilet.UserCode_CmdShit__NetworkIdentity__NetworkConnectionToClient(sender, sender.connectionToClient);

            // Prevent Original
            return false;
        }
    }
}
