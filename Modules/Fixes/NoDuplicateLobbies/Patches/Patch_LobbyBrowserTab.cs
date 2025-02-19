using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.NoDuplicateLobbies.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyBrowserTab
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyBrowserTab), nameof(LobbyBrowserTab.UpdateLobbyList))]
        private static void UpdateLobbyList_Prefix(LobbyBrowserTab __instance)
            => Patch_LobbyItem._lobbyList.Clear();
    }
}
