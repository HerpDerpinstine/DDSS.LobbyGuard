using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Fixes.NoDuplicateUsers.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyManager
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.UnRegisterPlayer))]
        private static void UnRegisterPlayer_Postfix(LobbyManager __instance,
            NetworkIdentity __0)
        {
            // Validate Server
            if (!NetworkServer.activeHost)
                return;

            // Get Lobby Player
            LobbyPlayer lobbyPlayer = __0.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return;

            // Remove Steam ID
            LobbyPlayer localPlayer = __instance.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected
                && (localPlayer.steamID != lobbyPlayer.steamID))
                LobbySecurity.RemoveValidSteamID(lobbyPlayer.steamID);
        }
    }
}