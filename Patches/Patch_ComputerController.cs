﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppComputer.Scripts.System;
using Il2CppMirror;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ComputerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdClick__Vector3__Int32))]
        private static bool InvokeUserCode_CmdClick__Vector3__Int32_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Read Button
            Vector3 clickpos = __1.SafeReadVector3();
            int buttonId = __1.SafeReadInt();
            if ((buttonId < 0)
                || (buttonId > 1))
                return false;

            // Run Game Command
            controller.UserCode_CmdClick__Vector3__Int32(clickpos, buttonId);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdCursorUp__Vector3))]
        private static bool InvokeUserCode_CmdCursorUp__Vector3_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Read Button
            Vector3 clickpos = __1.SafeReadVector3();

            // Run Game Command
            controller.UserCode_CmdCursorUp__Vector3(clickpos);

            // Prevent Original
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdKeyPressed__String))]
        private static bool InvokeUserCode_CmdKeyPressed__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Read String
            string keyCodeStr = __1.SafeReadString();
            if (keyCodeStr == null)
                return false;

            // Validate String Length
            int keyCodeStrLen = keyCodeStr.Length;
            if ((keyCodeStrLen < 1)
                || (keyCodeStrLen > 1))
                return false;

            // Run Game Command
            controller.UserCode_CmdKeyPressed__String(keyCodeStr);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdSyncCursor__Vector3))]
        private static bool InvokeUserCode_CmdSyncCursor__Vector3_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Read Cursor Position
            Vector3 cursorPos = __1.SafeReadVector3();

            // Run Game Command
            controller.UserCode_CmdSyncCursor__Vector3(cursorPos);

            // Prevent Original
            return false;
        }
    }
}
