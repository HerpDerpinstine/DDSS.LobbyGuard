using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_VoteBoxController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VoteBoxController), nameof(VoteBoxController.InvokeUserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
           NetworkBehaviour __0,
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Get VoteBoxController
            VoteBoxController box = __0.TryCast<VoteBoxController>();
            if (box == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, box.transform.position))
                return false;

            // Get Value
            __1.ReadNetworkIdentity();
            bool state = __1.ReadBool();

            // Run Game Command
            box.UserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, state, __2);

            // Prevent Original
            return false;
        }
    }
}
