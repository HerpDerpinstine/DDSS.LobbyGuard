using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.CameraProp;

namespace DDSS_LobbyGuard.Modules.Security.Player.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdReplacePlayer__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Validate Server
            if ((__2.identity == null)
                || __2.identity.WasCollected
                || !LobbyManager.instance.gameStarted)
                return false;

            // Validate Sender
            LobbyPlayer sender = __0.TryCast<LobbyPlayer>();
            if ((sender == null)
                || sender.WasCollected
                || ((sender.NetworkplayerController != null)
                    && !sender.NetworkplayerController.WasCollected))
                return false;

            // Run Game Command
            sender.UserCode_CmdReplacePlayer__NetworkConnectionToClient(__2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.ServerSetPlayerRole))]
        private static void ServerSetPlayerRole_Prefix(LobbyPlayer __instance, PlayerRole __0)
        {
            if ((__instance == null)
                || __instance.WasCollected)
                return;

            NetworkIdentity controllerNet = __instance.playerController;
            if ((controllerNet == null)
                || controllerNet.WasCollected)
                return;

            PlayerController controller = controllerNet.gameObject.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected
                || (controller.currentUsables == null)
                || controller.currentUsables.WasCollected
                || (controller.currentUsables.Count <= 0))
                return;

            if ((__0 == PlayerRole.Manager)
                || (__0 == PlayerRole.Janitor)
                || (__0 == PlayerRole.None))
            {
                if (__instance.subRole == SubRole.HrRep)
                    GameManager.instance.SelectNewHrRep();

                if (__instance.subRole == SubRole.Analyst)
                    GameManager.instance.SelectNewAnalyst();

                __instance.ServerSetSubRole(SubRole.None, true);
            }
        }
    }
}