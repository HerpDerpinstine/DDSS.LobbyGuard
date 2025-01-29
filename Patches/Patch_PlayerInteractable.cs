using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerInteractable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_PromoteToAssistant__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_PromoteToAssistant__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            if (!GameManager.instance.useAssistant)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            PlayerController controller = interact.playerController;
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer targetPlayer = controller.NetworklobbyPlayer;
            if ((targetPlayer == null)
                || targetPlayer.WasCollected
                || targetPlayer.IsGhost()
                || (targetPlayer.NetworkplayerRole == PlayerRole.Manager)
                || (targetPlayer.NetworksubRole == SubRole.Assistant))
                return false;

            // Validate Sender
            PlayerController senderController = sender.GetComponent<PlayerController>();
            if ((senderController == null)
                || senderController.WasCollected)
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = senderController.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            LobbyPlayer lobbyPlayer = senderController.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || (lobbyPlayer.NetworkplayerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            // Get Last Assistant
            LobbyPlayer lastAssistant = LobbyManager.instance.GetAssistantPlayer();

            // Run Game Command
            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
            {
                // Get Old Player
                LobbyPlayer oldPlayer = networkIdentity.GetComponent<LobbyPlayer>();
                if ((oldPlayer == null)
                    || (oldPlayer == targetPlayer)
                    || (oldPlayer.NetworksubRole != SubRole.Assistant))
                    continue;

                // Reset Role
                oldPlayer.ServerSetSubRole(SubRole.None);
            }

            bool wasHR = (targetPlayer.NetworksubRole == SubRole.HrRep);
            targetPlayer.ServerSetSubRole(SubRole.Assistant);
            if (GameManager.instance.NetworkuseHrRep && wasHR)
                GameManager.instance.SelectNewHrRep();

            if ((lastAssistant != null)
                && !lastAssistant.WasCollected)
                GameManager.instance.RpcSetAssistant(lastAssistant.netIdentity, targetPlayer.netIdentity);
            else
                GameManager.instance.RpcSetAssistant(null, targetPlayer.netIdentity);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdDemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdDemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            if (!GameManager.instance.useAssistant)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            PlayerController controller = interact.playerController;
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer targetPlayer = controller.NetworklobbyPlayer;
            if ((targetPlayer == null)
                || targetPlayer.WasCollected
                || targetPlayer.IsGhost()
                || (targetPlayer.NetworkplayerRole == PlayerRole.Manager)
                || (targetPlayer.NetworksubRole == SubRole.None))
                return false;

            // Validate Sender
            PlayerController senderController = sender.GetComponent<PlayerController>();
            if ((senderController == null)
                || senderController.WasCollected)
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = senderController.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            LobbyPlayer lobbyPlayer = senderController.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || (lobbyPlayer.NetworkplayerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            targetPlayer.ServerSetSubRole(SubRole.None);
            GameManager.instance.RpcSetAssistant(targetPlayer.netIdentity, null);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_AcceptHandShake__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_AcceptHandShake__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position, InteractionSecurity.MAX_DISTANCE_PLAYER)
                || !interact.HasPlayerRequestedHandShake(senderInteract))
                return false;

            interact.UserCode_AcceptHandShake__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdRequestHandShake__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdRequestHandShake__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position, InteractionSecurity.MAX_DISTANCE_PLAYER)
                || interact.HasPlayerRequestedHandShake(senderInteract))
                return false;

            // Reset Current Request
            if (senderInteract.NetworkthisPlayerRequestedHandshakeWith != null)
            {
                PlayerController component2 = senderInteract.NetworkthisPlayerRequestedHandshakeWith.GetComponent<PlayerController>();
                if (component2 != null)
                    component2.playerInteractable.UserCode_CmdResetHandShakeRequest__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            }

            // Apply New Request
            interact.NetworkplayerRequestedHandshakeWithThisPlayer = sender;
            senderInteract.NetworkthisPlayerRequestedHandshakeWith = interact.playerController.netIdentity;

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetHandShake__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdResetHandShake__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract != interact))
                return false;

            senderInteract.UserCode_CmdResetHandShake__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetOutgoingHandShake))]
        private static bool InvokeUserCode_CmdResetOutgoingHandShake_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract != interact))
                return false;

            // Reset Current Request
            if (senderInteract.NetworkthisPlayerRequestedHandshakeWith != null)
            {
                PlayerController component2 = senderInteract.NetworkthisPlayerRequestedHandshakeWith.GetComponent<PlayerController>();
                if (component2 != null)
                    component2.playerInteractable.UserCode_CmdResetHandShakeRequest__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            }

            // Reset Current Request
            senderInteract.UserCode_CmdResetOutgoingHandShake();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
