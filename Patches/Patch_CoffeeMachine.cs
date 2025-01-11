using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_CoffeeMachine
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CoffeeMachine), nameof(CoffeeMachine.InvokeUserCode_CmdMakeCoffee__NetworkIdentity))]
        private static bool InvokeUserCode_CmdMakeCoffee__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CoffeeMachine
            CoffeeMachine machine = __0.TryCast<CoffeeMachine>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, machine.transform.position))
                return false;

            if (machine.collectibles.Count <= 0)
                return false;

            Mug mug = null;
            foreach (var m in machine.collectibles.Keys)
            {
                mug = m.TryCast<Mug>();
                if (mug != null)
                    break;
            }
            if (mug == null)
                return false;

            if (mug.NetworkcoffeeAmount >= 1f)
                return false;

            // Run Game Command
            machine.UserCode_CmdMakeCoffee__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
