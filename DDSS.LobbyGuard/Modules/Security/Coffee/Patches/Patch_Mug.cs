using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Coffee.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Mug
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Mug), nameof(Mug.InvokeUserCode_CmdDrinkCoffee__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdDrinkCoffee__NetworkIdentity__NetworkConnectionToClient_Prefix(
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
            mug.UserCode_CmdDrinkCoffee__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            mug.ServerSetCoffeeAmount(0f);

            // Prevent Original
            return false;
        }
    }
}