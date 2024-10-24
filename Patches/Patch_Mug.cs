using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
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
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Mug
            Mug mug = __0.TryCast<Mug>();
            if ((mug == null)
                || (mug.coffeeAmount <= 0f))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, mug.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<Mug>()))
                return false;

            // Get Mug
            mug = collectible.TryCast<Mug>();
            if ((mug == null)
                || (mug.coffeeAmount == 0f))
                return false;

            // Run Game Command
            mug.UserCode_CmdDrinkCoffee__NetworkIdentity(sender);

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
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Value
            float amount = __1.ReadFloat();

            // Get Mug
            Mug mug = __0.TryCast<Mug>();
            if ((mug == null)
                || (mug.coffeeAmount == amount))
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, mug.transform.position))
                return false;

            // Check if should Fill or Drink
            if (amount == 1f)
            {
                // Validate Placement
                CollectibleHolder holder = mug.currentHolder;
                if ((holder == null)
                    || (holder.GetIl2CppType() != Il2CppType.Of<CoffeeMachine>()))
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
                if ((mug == null)
                    || (mug.coffeeAmount == amount))
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
