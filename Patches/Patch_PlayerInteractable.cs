using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Scripts;
using UnityEngine;

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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position))
                return false;

            // Get Last Assistant
            LobbyPlayer lastAssistant = LobbyManager.instance.GetAssistantPlayer();

            // Run Game Command
            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedLobbyPlayers)
            {
                // Get Old Player
                LobbyPlayer oldPlayer = networkIdentity.GetComponent<LobbyPlayer>();
                if ((oldPlayer == null)
                    || (oldPlayer.NetworksubRole != SubRole.Assistant))
                    continue;

                // Reset Role
                oldPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);
            }
            LobbyPlayer localPlayer = LobbyManager.instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer.NetworksubRole != SubRole.None))
                localPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);

            targetPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.Assistant);

            if ((lastAssistant != null)
                && !lastAssistant.WasCollected)
                GameManager.instance.RpcSetAssistant(lastAssistant.netIdentity, targetPlayer.netIdentity);
            else
                GameManager.instance.RpcSetAssistant(null, targetPlayer.netIdentity);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_DemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_DemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position))
                return false;

            targetPlayer.UserCode_CmdSetSubRole__SubRole(SubRole.None);
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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position)
                || !senderInteract.HasPlayerRequestedHandShake(interact))
                return false;

            interact.UserCode_AcceptHandShake__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdRequestHandShake__NetworkIdentity))]
        private static bool InvokeUserCode_CmdRequestHandShake__NetworkIdentity_Prefix(NetworkBehaviour __0,
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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position)
                || interact.HasPlayerRequestedHandShake(senderInteract))
                return false;

            // Reset Current Request
            if (senderInteract.NetworkthisPlayerRequestedHandshakeWith != null)
            {
                PlayerController component2 = senderInteract.NetworkthisPlayerRequestedHandshakeWith.GetComponent<PlayerController>();
                if (component2 != null)
                    component2.playerInteractable.UserCode_CmdResetHandShakeRequest__NetworkIdentity(sender);
            }

            // Apply New Request
            interact.NetworkplayerRequestedHandshakeWithThisPlayer = sender;
            senderInteract.NetworkthisPlayerRequestedHandshakeWith = interact.playerController.netIdentity;

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetHandShake))]
        private static bool InvokeUserCode_CmdResetHandShake_Prefix(NetworkBehaviour __0,
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

            senderInteract.UserCode_CmdResetHandShake();

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
                    component2.playerInteractable.UserCode_CmdResetHandShakeRequest__NetworkIdentity(sender);
            }

            // Reset Current Request
            senderInteract.UserCode_CmdResetOutgoingHandShake();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity))]
        private static bool InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
