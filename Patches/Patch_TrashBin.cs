using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.TrashBin;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_TrashBin
    {
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
            float distance = Vector3.Distance(sender.transform.position, trashcan.transform.position);
            if (distance > MelonMain.MAX_INTERACTION_DISTANCE)
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
    }
}
