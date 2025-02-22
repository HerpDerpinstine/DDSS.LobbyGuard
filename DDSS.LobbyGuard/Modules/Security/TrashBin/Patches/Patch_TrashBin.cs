using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.FireEx;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.TrashBin.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_TrashBin
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.TrashBin.TrashBin), nameof(Il2CppProps.TrashBin.TrashBin.InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get TrashBin
            Il2CppProps.TrashBin.TrashBin trashcan = __0.TryCast<Il2CppProps.TrashBin.TrashBin>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender == null
                || sender.WasCollected)
                return false;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if (controller == null
                || controller.WasCollected)
                return false;

            // Get Player
            LobbyPlayer player = controller.NetworklobbyPlayer;
            if (player == null
                || player.WasCollected
                || player.IsGhost())
                return false;

            if (!InteractionSecurity.IsWithinRange(sender.transform.position, trashcan.transform.position))
                return false;

            // Get Values
            __1.SafeReadNetworkIdentity();
            bool enabled = __1.SafeReadBool();

            // Validate TrashBin
            if (trashcan.isOnFire == enabled)
                return false;

            // Check for Extinguishing
            if (enabled)
            {
                // Get Player
                if (!InteractionSecurity.IsSlacker(player))
                    return false;
            }
            else
            {
                // Validate Placement
                Collectible collectible = controller.GetCurrentCollectible();
                if (collectible == null
                    || collectible.GetIl2CppType() != Il2CppType.Of<FireExController>())
                    return false;

                // Get FireExController
                FireExController fireEx = collectible.TryCast<FireExController>();
                if (fireEx == null)
                    return false;
            }

            // Run Game Command
            trashcan.UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, enabled, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.TrashBin.TrashBin), nameof(Il2CppProps.TrashBin.TrashBin.InvokeUserCode_CmdEmptyBin__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEmptyBin__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get TrashBin
            Il2CppProps.TrashBin.TrashBin trashcan = __0.TryCast<Il2CppProps.TrashBin.TrashBin>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, trashcan.transform.position))
                return false;

            // Run Game Command
            trashcan.UserCode_CmdEmptyBin__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

    }
}
