﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Security.Manager.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_VoteBoxController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VoteBoxController), nameof(VoteBoxController.InvokeUserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            if (!GameManager.instance.replaceManagerVoting)
                return false;

            // Get VoteBoxController
            VoteBoxController box = __0.TryCast<VoteBoxController>();
            if ((box == null)
                || box.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, box))
                return false;

            // Validate Role
            PlayerRole role = sender.GetPlayerRole();
            if ((role == PlayerRole.None)
                || (role == PlayerRole.Manager)
                || (!Extras.MoreRoleSettings.ModuleConfig.Instance.AllowJanitorsToVote.Value && (role == PlayerRole.Janitor)))
                return false;

            // Get Value
            __1.SafeReadNetworkIdentity();
            bool state = __1.SafeReadBool();

            // Run Game Command
            box.UserCode_CmdVote__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, state, __2);

            // Prevent Original
            return false;
        }
    }
}
