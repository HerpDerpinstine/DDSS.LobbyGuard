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
        private static bool InvokeUserCode_CmdDrinkWater__NetworkIdentity_Prefix(
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
            mug.UserCode_CmdSetWaterAmount__Single(0f);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WaterCupController), nameof(WaterCupController.InvokeUserCode_CmdSetWaterAmount__Single))]
        private static bool InvokeUserCode_CmdSetWaterAmount__Single_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
