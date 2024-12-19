using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.PlayerEffects;
using Il2CppProps.Scripts;
using System;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerEffectController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerEffectController), nameof(PlayerEffectController.Update))]
        private static void Update_Postfix(PlayerEffectController __instance)
        {
            // Check for Server
            if (!__instance.isServer)
                return;

            // Validate Effect has Ended
            int normalEffect = (int)PlayerEffects.Normal;
            if ((__instance.NetworktargetState == normalEffect)
                || (__instance.currentEffect == null)
                || (__instance.currentEffect.timeCounter > 0))
                return;

            // Manually End Effect
            __instance.UserCode_CmdSetEffect__Int32__Single(normalEffect, 0f);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerEffectController), nameof(PlayerEffectController.InvokeUserCode_CmdSetEffect__Int32__Single))]
        private static bool InvokeUserCode_CmdSetEffect__Int32__Single_Prefix(
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Get PlayerEffectController
            PlayerEffectController controller = sender.GetComponent<PlayerEffectController>();
            if (controller == null)
                return false;

            // Validate Effect
            int effectValue = __1.SafeReadInt();
            if (!Enum.IsDefined(typeof(PlayerEffects), effectValue)
                || (effectValue == controller.NetworktargetState))
                return false;

            // Validate Duration
            float duration = __1.SafeReadFloat();
            if ((duration <= 0)
                || (duration > 180))
                return false;

            // Parse Effect
            PlayerEffects requestedEffect = (PlayerEffects)effectValue;
            if (requestedEffect == PlayerEffects.SpeedBoost)
            {
                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<Mug>()))
                    return false;

                // Get Mug
                Mug mug = collectible.TryCast<Mug>();
                if ((mug == null)
                    || (mug.coffeeAmount != 0f))
                    return false;
            }
            else if (requestedEffect == PlayerEffects.Drunk) 
            {
                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<BeerController>()))
                    return false;

                // Get BeerController
                BeerController beer = collectible.TryCast<BeerController>();
                if ((beer == null)
                    || !beer.NetworkisEmpty)
                    return false;
            }

            // Run Game Command
            controller.UserCode_CmdSetEffect__Int32__Single(effectValue, duration);

            // Prevent Original
            return false;
        }
    }
}
