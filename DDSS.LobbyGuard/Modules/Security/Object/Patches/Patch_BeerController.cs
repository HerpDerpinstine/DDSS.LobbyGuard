﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_BeerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BeerController), nameof(BeerController.InvokeUserCode_CmdDrinkBeer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdDrinkBeer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get BeerController
            BeerController beer = __0.TryCast<BeerController>();
            if ((beer == null)
                || beer.NetworkisEmpty)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, beer))
                return false;

            // Validate Placement
            Il2CppProps.Scripts.Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<BeerController>()))
                return false;

            // Get Document
            beer = collectible.TryCast<BeerController>();
            if ((beer == null)
                || beer.NetworkisEmpty)
                return false;

            // Run Game Command
            beer.UserCode_CmdDrinkBeer__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}