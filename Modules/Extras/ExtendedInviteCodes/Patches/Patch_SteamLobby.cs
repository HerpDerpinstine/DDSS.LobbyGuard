using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.ExtendedInviteCodes.Patches
{
    [HarmonyPatch]
    internal class Patch_SteamLobby
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.GetRandomLobbyCode))]
        private static bool GetRandomLobbyCode_Prefix(ref string __result)
        {
            // Generate New Code
            __result = InviteCodeSecurity.GenerateNewCode();

            // Prevent Original
            return false;
        }
    }
}