using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.TrashBin;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_TrashBin
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.RpcEnableFire))]
        private static void RpcEnableFire_Postfix(TrashBin __instance)
        {
            // TrashBin Security
            TrashBinSecurity.OnEnableFireEnd(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.UserCode_CmdEnableFire__NetworkIdentity__Boolean))]
        private static bool UserCode_CmdEnableFire__NetworkIdentity__Boolean_Prefix(TrashBin __instance, NetworkIdentity __0, bool __1)
        {
            // TrashBin Security
            TrashBinSecurity.OnEnableFireBegin(__0, __instance, __1);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(TrashBin), nameof(TrashBin.InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean))]
        private static bool InvokeUserCode_CmdEnableFire__NetworkIdentity__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get TrashBin
            TrashBin trashcan = __0.TryCast<TrashBin>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Values
            __1.ReadNetworkIdentity();
            bool enabled = __1.ReadBool();

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, trashcan.transform.position))
                return false;

            // Check for Enable
            if (enabled 
                && !sender.isServer)
            {
                // Validate Slacker Role
                LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || (player.playerRole != PlayerRole.Slacker))
                    return false;
            }

            // Run Game Command
            trashcan.UserCode_CmdEnableFire__NetworkIdentity__Boolean(sender, enabled);

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