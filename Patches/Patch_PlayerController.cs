using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdSpank__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSpank__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0, NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            PlayerController target = __0.TryCast<PlayerController>();
            if ((target == null)
                || target.WasCollected
                || target != controller)
                return false;

            // Run Game Command
            controller.UserCode_CmdSpank__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdSetLocalHeadRot__Quaternion))]
        private static bool InvokeUserCode_CmdSetLocalHeadRot__Quaternion(NetworkBehaviour __0, NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            PlayerController target = __0.TryCast<PlayerController>();
            if ((target == null)
                || target.WasCollected
                || target != controller)
                return false;

            // Get Rot
            Quaternion rot = __1.SafeReadQuaternion();

            // Run Game Command
            controller.UserCode_CmdSetLocalHeadRot__Quaternion(rot);

            // Prevent Original
            return false;
        }
    }
}
