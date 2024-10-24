using HarmonyLib;
using Il2CppSteamworks;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_SteamMatchmaking
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.RequestLobbyList))]
        private static void RequestLobbyList_Prefix()
        {
            Patch_LobbyItem._lobbyList.Clear();
        }
    }
}