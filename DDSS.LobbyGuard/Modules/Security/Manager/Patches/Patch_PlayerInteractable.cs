using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Manager.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
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
                || (targetPlayer.playerRole == PlayerRole.Manager)
                || (targetPlayer.subRole != SubRole.None))
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
                || (lobbyPlayer.playerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            if (ModuleConfig.Instance.AutomaticallyDemotePreviousAssistant.Value)
                GameManager.instance.ServerSetAssistant(null);
            GameManager.instance.ServerSetAssistant(targetPlayer.netIdentity);

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
                || (targetPlayer.playerRole == PlayerRole.Manager)
                || (targetPlayer.subRole != SubRole.Assistant))
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
                || (lobbyPlayer.playerRole != PlayerRole.Manager))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            GameManager.instance.ServerSetAssistant(null);

            // Prevent Original 
            return false;
        }
    }
}
