using HarmonyLib;
using Il2Cpp;
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
    }
}