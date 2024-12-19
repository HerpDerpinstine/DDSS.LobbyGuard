using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.FireEx;
using Il2CppProps.Scripts;
using Il2CppProps.TrashBin;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_TrashBin
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.RpcEnableFire))]
        private static void RpcEnableFire_Postfix(TrashBin __instance, bool __1)
        {
            // TrashBin Security
            __instance.isOnFire = __1;
            if (!__1)
                TrashBinSecurity.OnEnableFireEnd(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(TrashBin __instance, NetworkIdentity __0, bool __1)
        {
            // TrashBin Security
            __instance.isOnFire = __1;
            TrashBinSecurity.OnEnableFireBegin(__0, __instance, __1);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get TrashBin
            TrashBin trashcan = __0.TryCast<TrashBin>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
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
                LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected
                    || !InteractionSecurity.IsSlacker(player))
                    return false;
            }
            else
            {
                // Validate Placement
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<FireExController>()))
                    return false;

                // Get FireExController
                FireExController fireEx = collectible.TryCast<FireExController>();
                if (fireEx == null)
                    return false;

                // Validate FireExController
                if (!InteractionSecurity.IsWithinRange(fireEx.transform.position, trashcan.transform.position))
                    return false;
            }

            // Run Game Command
            trashcan.UserCode_CmdEnableFire__NetworkIdentity__Boolean__NetworkConnectionToClient(sender, enabled, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.InvokeUserCode_CmdEmptyBin__NetworkIdentity))]
        private static bool InvokeUserCode_CmdEmptyBin__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get TrashBin
            TrashBin trashcan = __0.TryCast<TrashBin>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, trashcan.transform.position))
                return false;

            // Run Game Command
            trashcan.UserCode_CmdEmptyBin__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}