﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CatController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatController), nameof(CatController.InvokeUserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CatController
            CatController cat = __0.TryCast<CatController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, cat))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<CatFoodController>()))
                return false;

            // Get CatFoodController
            CatFoodController food = collectible.TryCast<CatFoodController>();
            if (food == null)
                return false;

            // Run Game Command
            cat.UserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(CatController), nameof(CatController.InvokeUserCode_CmdPet__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPet__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CatController
            CatController cat = __0.TryCast<CatController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, cat))
                return false;

            // Run Game Command
            cat.UserCode_CmdPet__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}