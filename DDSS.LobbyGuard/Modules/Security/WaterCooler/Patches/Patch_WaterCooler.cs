﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.WaterCooler.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_WaterCooler
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2Cpp.WaterCooler), nameof(Il2Cpp.WaterCooler.InvokeUserCode_CmdGrabCup__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdGrabCup__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WaterCooler
            Il2Cpp.WaterCooler cooler = __0.TryCast<Il2Cpp.WaterCooler>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, cooler))
                return false;

            // Get Collectible
            Collectible collectible = cooler.cupPrefab.GetComponent<Collectible>();
            if ((collectible == null)
                || collectible.WasCollected
                || !InteractionSecurity.CanUseUsable(sender, collectible))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(collectible.NetworkinteractableName, InteractionSecurity.MAX_WATERCUPS))
                return false;

            // Run Game Command
            cooler.UserCode_CmdGrabCup__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2Cpp.WaterCooler), nameof(Il2Cpp.WaterCooler.InvokeUserCode_CmdFillCup__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFillCup__NetworkIdentity__NetworkConnectionToClient(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WaterCooler
            Il2Cpp.WaterCooler cooler = __0.TryCast<Il2Cpp.WaterCooler>();
            if (cooler == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, cooler))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<WaterCupController>()))
                return false;

            // Get Mug
            WaterCupController mug = collectible.TryCast<WaterCupController>();
            if ((mug == null)
                || (mug.NetworkwaterAmount >= 1f))
                return false;

            // Run Game Command
            cooler.UserCode_CmdFillCup__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            mug.ServerSetWaterAmount(1f);

            // Prevent Original
            return false;
        }
    }
}