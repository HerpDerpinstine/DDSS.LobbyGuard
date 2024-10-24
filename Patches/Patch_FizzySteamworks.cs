using HarmonyLib;
using Il2CppMirror.FizzySteam;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_FizzySteamworks
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ClientConnect), typeof(string))]
        private static void ClientConnect_string_Prefix(FizzySteamworks __instance)
        {
            // Enforce Next Gen Networking
            __instance.UseNextGenSteamNetworking = true;
            __instance.AllowSteamRelay = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ClientConnect), typeof(Il2CppSystem.Uri))]
        private static void ClientConnect_Uri_Prefix(FizzySteamworks __instance)
        {
            // Enforce Next Gen Networking
            __instance.UseNextGenSteamNetworking = true;
            __instance.AllowSteamRelay = true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FizzySteamworks), nameof(FizzySteamworks.ServerStart))]
        private static void ServerStart_Prefix(FizzySteamworks __instance)
        {
            // Enforce Next Gen Networking
            __instance.UseNextGenSteamNetworking = true;
            __instance.AllowSteamRelay = true;
        }
    }
}
