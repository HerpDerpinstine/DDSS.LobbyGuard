using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
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
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.isServer)
                return true;

            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || (lobbyPlayer.NetworkplayerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            interact.UserCode_PromoteToAssistant__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_DemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_DemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.isServer)
                return true;

            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Validate Interactable
            PlayerInteractable senderInteract = controller.playerInteractable;
            if ((senderInteract == null)
                || senderInteract.WasCollected
                || (senderInteract == interact))
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || (lobbyPlayer.NetworkplayerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            interact.UserCode_DemoteFromAssistant__NetworkIdentity__NetworkConnectionToClient(sender, __2);

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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, 
                interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
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
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, 
                interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            interact.UserCode_CmdRequestHandShake__NetworkIdentity(sender);

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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            senderInteract.UserCode_CmdResetHandShake();

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerInteractable), nameof(PlayerInteractable.InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity))]
        private static bool InvokeUserCode_CmdResetHandShakeRequest__NetworkIdentity_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.isServer)
                return true;

            // Get PlayerInteractable
            PlayerInteractable interact = __0.TryCast<PlayerInteractable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            senderInteract.UserCode_CmdResetHandShakeRequest__NetworkIdentity(sender);

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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position,
                InteractionSecurity.MAX_PLAYER_INTERACT_DISTANCE))
                return false;

            senderInteract.UserCode_CmdResetOutgoingHandShake();

            // Prevent Original
            return false;
        }
    }
}
