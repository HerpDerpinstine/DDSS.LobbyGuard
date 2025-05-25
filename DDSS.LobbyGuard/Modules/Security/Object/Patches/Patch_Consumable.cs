using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Consumable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Consumable), nameof(Consumable.InvokeUserCode_CmdConsume__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdConsume__NetworkIdentity__NetworkConnectionToClient_Prefix(
          NetworkBehaviour __0,
          NetworkConnectionToClient __2)
        {
            // Get Consumable
            Consumable food = __0.TryCast<Consumable>();
            if (food == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return true;

            LobbyPlayer player = controller.NetworklobbyPlayer;
            if ((player == null)
                || player.WasCollected
                || player.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, food.transform.position))
                return false;

            // Validate Placement
            Il2CppProps.Scripts.Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get Consumable
            food = collectible.TryCast<Consumable>();
            if (food == null)
                return false;

            // Run Game Command
            food.UserCode_CmdConsume__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}