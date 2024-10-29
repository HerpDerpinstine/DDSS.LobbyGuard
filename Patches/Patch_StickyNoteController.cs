﻿using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    [HarmonyPatch]
    internal class Patch_StickyNoteController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StickyNoteController), nameof(StickyNoteController.InvokeUserCode_CmdSetText__String__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetText__String__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get StickyNoteController
            StickyNoteController __instance = __0.TryCast<StickyNoteController>();
            if (__instance == null)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, __instance.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<StickyNoteController>()))
                return false;

            // Get StickyNoteController
            __instance = collectible.TryCast<StickyNoteController>();
            if (__instance == null)
                return false;

            // Get Value
            string text = __1.ReadString();
            if (string.IsNullOrEmpty(text)
                || string.IsNullOrWhiteSpace(text))
                return false;

            // Run Game Command
            text = text.RemoveRichText();
            if (text.Length > 100)
                text = text.Substring(0, 100);
            __instance.RpcSetText(text);

            // Prevent Original
            return false;
        }
    }
}