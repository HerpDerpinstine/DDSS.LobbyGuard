using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard
{
    internal class Patch_Consumable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Consumable), nameof(Consumable.InvokeUserCode_CmdConsume__NetworkIdentity))]
        private static bool InvokeUserCode_CmdConsume__NetworkIdentity_Prefix(
           NetworkBehaviour __0,
           NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Consumable
            Consumable food = __0.TryCast<Consumable>();
            if (food == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, food.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<Consumable>()))
                return false;

            // Get Consumable
            food = collectible.TryCast<Consumable>();
            if (food == null)
                return false;

            // Run Game Command
            food.UserCode_CmdConsume__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}