using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Fixes.RemoveRichText.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.Networkusername), MethodType.Setter)]
        private static void Networkusername_set_Prefix(ref string __0)
        {
            __0 = __0.RemoveRichText();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.NetworksteamUsername), MethodType.Setter)]
        private static void NetworksteamUsername_set_Prefix(ref string __0)
        {
            __0 = __0.RemoveRichText();
        }
    }
}
