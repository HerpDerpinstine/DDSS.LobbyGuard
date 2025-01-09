using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppDissonance.Integrations.MirrorIgnorance;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_MirrorIgnorancePlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MirrorIgnorancePlayer), nameof(MirrorIgnorancePlayer.InvokeUserCode_CmdSetPlayerName__String))]
        private static bool InvokeUserCode_CmdSetPlayerName__String_Prefix(NetworkBehaviour __0, NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Sender
            MirrorIgnorancePlayer controller = sender.GetComponent<MirrorIgnorancePlayer>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Validate Target
            MirrorIgnorancePlayer target = __0.TryCast<MirrorIgnorancePlayer>();
            if ((target == null)
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
