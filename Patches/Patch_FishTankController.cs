using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FishTankController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FishTankController), nameof(FishTankController.InvokeUserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkIdentity __0,
            NetworkConnectionToClient __2)
        {
            // Get FishTankController
            FishTankController tank = __0.TryCast<FishTankController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, tank.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate InkCartridge
            InkCartridge ink = collectible.TryCast<InkCartridge>();
            if ((ink == null)
                || ink.WasCollected)
                return false;

            tank.UserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FishTankController), nameof(FishTankController.InvokeUserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkIdentity __0,
            NetworkConnectionToClient __2)
        {
            // Get FishTankController
            FishTankController tank = __0.TryCast<FishTankController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, tank.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate FishFoodController
            FishFoodController food = collectible.TryCast<FishFoodController>();
            if ((food == null)
                || food.WasCollected)
                return false;

            tank.UserCode_CmdFeed__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
