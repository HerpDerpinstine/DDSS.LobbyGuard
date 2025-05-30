﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Stereo.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_StereoController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StereoController), nameof(StereoController.InvokeUserCode_CmdStop__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStop__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StereoController
            StereoController stereo = __0.TryCast<StereoController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, stereo))
                return false;

            // Run Game Command
            stereo.UserCode_CmdStop__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StereoController), nameof(StereoController.InvokeUserCode_CmdPlayCD__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPlay__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StereoController
            StereoController stereo = __0.TryCast<StereoController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, stereo))
                return false;

            // Run Game Command
            stereo.UserCode_CmdPlayCD__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}