using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Mug
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mug), nameof(Mug.InvokeUserCode_CmdDrinkCoffee__NetworkIdentity))]
        private static bool InvokeUserCode_CmdDrinkCoffee__NetworkIdentity_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Get Mug
            Mug mug = __0.TryCast<Mug>();
            if (mug == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<Mug>()))
                return false;

            // Get Mug
            mug = collectible.TryCast<Mug>();
            if (mug == null)
                return false;

            // Run Game Command
            mug.UserCode_CmdDrinkCoffee__NetworkIdentity(sender);
            mug.UserCode_CmdSetCoffeeAmount__Single(0f);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mug), nameof(Mug.InvokeUserCode_CmdSetCoffeeAmount__Single))]
        private static bool InvokeUserCode_CmdSetCoffeeAmount__Single_Prefix(
           NetworkBehaviour __0,
           NetworkReader __1,
           NetworkConnectionToClient __2)
        {
            // Get Value
            float amount = __1.SafeReadFloat();

            // Get Mug
            Mug mug = __0.TryCast<Mug>();
            if ((mug == null)
                || (mug.NetworkcoffeeAmount == amount))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost())
                return false;

            // Check if should Fill or Drink
            if (amount == 1f)
            {
                // Validate Placement
                CollectibleHolder holder = mug.currentHolder;
                if ((holder == null)
                    || (holder.GetIl2CppType() != Il2CppType.Of<CoffeeMachine>())
                    || !InteractionSecurity.IsWithinRange(sender.transform.position, holder.transform.position))
                    return false;
            }
            else if (amount == 0f)
            {
                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<Mug>()))
                    return false;

                // Get Mug
                mug = collectible.TryCast<Mug>();
                if (mug == null)
                    return false;
            }
            else
                return false;

            // Run Game Command
            mug.UserCode_CmdSetCoffeeAmount__Single(amount);

            // Prevent Original
            return false;
        }
    }
}
