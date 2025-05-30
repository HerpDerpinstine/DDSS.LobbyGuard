﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_StorageBox
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StorageBox), nameof(StorageBox.InvokeUserCode_CmdGrabObject__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdGrabObject__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get StorageBox
            StorageBox box = __0.TryCast<StorageBox>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, box))
                return false;

            // Get Collectible
            Collectible collectible = box.objectPrefab.GetComponent<Collectible>();
            if ((collectible == null)
                || collectible.WasCollected
                || !InteractionSecurity.CanUseUsable(sender, collectible))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(collectible.NetworkinteractableName, box.maxItemCount))
                return false;

            // Run Game Command
            box.UserCode_CmdGrabObject__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}