using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.CameraProp;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.ServerSetPlayerRole))]
        private static void ServerSetPlayerRole_Prefix(LobbyPlayer __instance, PlayerRole __0)
        {
            if ((__instance == null)
                || __instance.WasCollected
                || (__0 != PlayerRole.None))
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

            foreach (var usable in controller.currentUsables)
            {
                // Drop It
                usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(controllerNet, controller.connectionToClient);

                // Manually Drop It on Server's Client
                usable.UserCode_RpcOnStopUse__NetworkIdentity(controllerNet);

                // Change Ownership
                usable.netIdentity.RemoveClientAuthority();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.ServerSetSubRole))]
        private static void ServerSetSubRole_Prefix(LobbyPlayer __instance, SubRole __0)
        {
            if ((__instance == null)
                || __instance.WasCollected
                || (__instance.subRole != SubRole.Assistant))
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

            var cameraType = Il2CppType.Of<CameraPropController>();
            foreach (var usable in controller.currentUsables)
            {
                var usableType = usable.GetIl2CppType();
                if (usableType != cameraType)
                    continue;

                // Drop It
                usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(controllerNet, controller.connectionToClient);

                // Manually Drop It on Server's Client
                usable.UserCode_RpcOnStopUse__NetworkIdentity(controllerNet);

                // Change Ownership
                usable.netIdentity.RemoveClientAuthority();
            }
        }
    }
}