using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.StartHost))]
        private static void StartHost_Prefix()
        {
            // Check if the setting is on
            if (!ConfigHandler.Lobby.ExtendedInviteCodes.Value)
                return;

            // Check if already has Invite Code
            if (!string.IsNullOrEmpty(SteamLobby.requestedLobbyCode)
                && !string.IsNullOrWhiteSpace(SteamLobby.requestedLobbyCode))
                return;

            // Generate New Code
            SteamLobby.requestedLobbyCode = InviteCodeSecurity.GenerateNewCode();
        }
    }
}
