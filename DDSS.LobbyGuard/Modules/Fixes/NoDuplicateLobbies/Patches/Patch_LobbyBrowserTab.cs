using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateLobbies.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyBrowserTab
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyBrowserTab), nameof(LobbyBrowserTab.UpdateTab))]
        private static void UpdateTab_Prefix(LobbyBrowserTab __instance)
            => Patch_LobbyItem._lobbyList.Clear();
    }
}
