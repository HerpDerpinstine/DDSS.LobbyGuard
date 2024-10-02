using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_NetworkManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.StartHost))]
        private static void StartHost_Prefix()
        {
            // Check if already has Invite Code
            if (!string.IsNullOrEmpty(SteamLobby.requestedLobbyCode)
                && !string.IsNullOrWhiteSpace(SteamLobby.requestedLobbyCode))
                return;

            // Generate New Code
            SteamLobby.requestedLobbyCode = InviteCodeSecurity.GenerateNewCode();
        }
    }
}
