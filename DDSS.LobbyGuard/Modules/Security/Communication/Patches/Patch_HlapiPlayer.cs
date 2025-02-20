using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppDissonance.Integrations.UNet_HLAPI;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Communication.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_HlapiPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HlapiPlayer), nameof(HlapiPlayer.InvokeUserCode_CmdSetPlayerName__String))]
        private static bool InvokeUserCode_CmdSetPlayerName__String_Prefix(NetworkBehaviour __0, NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender == null
                || sender.WasCollected)
                return false;

            // Validate Sender
            HlapiPlayer controller = sender.GetComponent<HlapiPlayer>();
            if (controller == null
                || controller.WasCollected)
                return false;

            // Validate Target
            HlapiPlayer target = __0.TryCast<HlapiPlayer>();
            if (target == null
                || target.WasCollected
                || target != controller)
                return false;

            // Validate Name
            string name = __1.SafeReadString();
            if (string.IsNullOrEmpty(name)
                || string.IsNullOrWhiteSpace(name))
                return false;

            // Run Game Command
            controller.UserCode_CmdSetPlayerName__String(name);

            // Prevent Original
            return false;
        }
    }
}
