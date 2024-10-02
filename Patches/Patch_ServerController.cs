using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.ServerRack;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_ServerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.InvokeUserCode_SetConnectionEnabled__NetworkIdentity__Boolean))]
        private static bool InvokeUserCode_SetConnectionEnabled__NetworkIdentity__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get ServerController
            ServerController server = __0.TryCast<ServerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Values
            __1.ReadNetworkIdentity();
            bool enabled = __1.ReadBool();

            // Validate Distance
            float distance = Vector3.Distance(sender.transform.position, server.transform.position);
            if (distance > MelonMain.MAX_INTERACTION_DISTANCE)
                return false;

            // Check for Disable
            if (!enabled 
                && !sender.isServer)
            {
                // Validate Slacker Role
                LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || (player.playerRole != PlayerRole.Slacker))
                    return false;
            }

            // Run Game Command
            server.UserCode_SetConnectionEnabled__NetworkIdentity__Boolean(sender, enabled);

            // Prevent Original
            return false;
        }
    }
}
