using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Security.Coffee.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CoffeeMachine
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2Cpp.CoffeeMachine), nameof(Il2Cpp.CoffeeMachine.InvokeUserCode_CmdMakeCoffee__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdMakeCoffee__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get CoffeeMachine
            Il2Cpp.CoffeeMachine machine = __0.TryCast<Il2Cpp.CoffeeMachine>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, machine))
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
            machine.UserCode_CmdMakeCoffee__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}