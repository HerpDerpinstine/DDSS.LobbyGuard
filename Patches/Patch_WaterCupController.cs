using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_WaterCupController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WaterCupController), nameof(WaterCupController.InvokeUserCode_CmdDrinkWater__NetworkIdentity))]
        private static bool InvokeUserCode_CmdDrinkCoffee__NetworkIdentity_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Get WaterCupController
            WaterCupController mug = __0.TryCast<WaterCupController>();
            if (mug == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<WaterCupController>()))
                return false;

            // Get Mug
            mug = collectible.TryCast<WaterCupController>();
            if (mug == null)
                return false;

            // Run Game Command
            mug.UserCode_CmdDrinkWater__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WaterCupController), nameof(WaterCupController.UserCode_CmdSetWaterAmount__Single))]
        private static bool InvokeUserCode_CmdSetCoffeeAmount__Single_Prefix(
           NetworkBehaviour __0,
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Get Value
            float amount = __1.SafeReadFloat();

            // Get Mug
            WaterCupController mug = __0.TryCast<WaterCupController>();
            if (mug == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost())
                return false;

            // Check if should Fill or Drink
            if ((amount == 1f)
                || (amount == 0f))
            {
                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<WaterCupController>()))
                    return false;

                // Get Mug
                mug = collectible.TryCast<WaterCupController>();
                if (mug == null)
                    return false;
            }
            else
                return false;

            // Run Game Command
            mug.UserCode_CmdSetWaterAmount__Single(amount);

            // Prevent Original
            return false;
        }
    }
}
