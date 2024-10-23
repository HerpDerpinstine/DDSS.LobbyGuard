﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
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

            // Validate State
            if (!StereoSecurity.GetState(stereo))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, stereo.transform.position))
                return false;

            // Run Game Command
            StereoSecurity.ApplyState(stereo, false);
            stereo.UserCode_CmdStop__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StereoController), nameof(StereoController.InvokeUserCode_CmdPlay__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPlay__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StereoController
            StereoController stereo = __0.TryCast<StereoController>();

            // Validate State
            if (StereoSecurity.GetState(stereo))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, stereo.transform.position))
                return false;

            // Run Game Command
            StereoSecurity.ApplyState(stereo, true);
            stereo.UserCode_CmdPlay__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StereoController), nameof(StereoController.InvokeUserCode_CmdPlayCustom__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPlayCustom__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StereoController
            StereoController stereo = __0.TryCast<StereoController>();

            // Validate State
            if (StereoSecurity.GetState(stereo))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, stereo.transform.position))
                return false;

            // Run Game Command
            StereoSecurity.ApplyState(stereo, true);
            stereo.UserCode_CmdPlayCustom__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StereoController), nameof(StereoController.InvokeUserCode_CmdPlayCD__Int32))]
        private static bool InvokeUserCode_CmdPlayCD__Int32_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StereoController
            StereoController stereo = __0.TryCast<StereoController>();

            // Validate State
            if (StereoSecurity.GetState(stereo))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, stereo.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<CDCase>()))
                return false;

            // Get CDCase
            CDCase cd = collectible.TryCast<CDCase>();
            if (cd == null)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(cd.transform.position, stereo.transform.position))
                return false;

            // Validate Song Index
            int songId = cd.songIndex;
            if ((songId < 0)
                || (songId >= stereo.songs.Count))
                return false;

            // Run Game Command
            StereoSecurity.ApplyState(stereo, true);
            stereo.UserCode_CmdPlayCD__Int32(cd.songIndex);

            // Prevent Original
            return false;
        }
    }
}