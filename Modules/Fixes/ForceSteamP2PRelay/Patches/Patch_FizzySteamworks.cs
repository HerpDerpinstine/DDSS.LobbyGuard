using HarmonyLib;
using Il2CppMirror.FizzySteam;

namespace DDSS_LobbyGuard.ForceSteamP2PRelay.Patches
{
    [HarmonyPatch]
    internal class Patch_FizzySteamworks
    {
        private static void ForceEnableRelay(FizzySteamworks fizzy)
        {
            fizzy.UseNextGenSteamNetworking = true;
            fizzy.AllowSteamRelay = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ClientConnect), typeof(string))]
        private static void ClientConnect_string_Prefix(FizzySteamworks __instance)
            => ForceEnableRelay(__instance);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ClientConnect), typeof(Il2CppSystem.Uri))]
        private static void ClientConnect_Uri_Prefix(FizzySteamworks __instance)
            => ForceEnableRelay(__instance);

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ServerStart))]
        private static void ServerStart_Prefix(FizzySteamworks __instance)
            => ForceEnableRelay(__instance);
    }
}
