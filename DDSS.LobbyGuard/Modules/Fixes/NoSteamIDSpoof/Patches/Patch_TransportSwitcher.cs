using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.Fixes.NoSteamIDSpoof.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_TransportSwitcher
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(TransportSwitcher), nameof(TransportSwitcher.SwitchTransportToKCP))]
        private static void SwitchTransportToKCP_Postfix()
        {
            ModuleMain.IsSteamLobby = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TransportSwitcher), nameof(TransportSwitcher.SwitchTransportToFizzySteamworks))]
        private static void SwitchTransportToFizzySteamworks_Postfix()
        {
            ModuleMain.IsSteamLobby = true;
        }
    }
}