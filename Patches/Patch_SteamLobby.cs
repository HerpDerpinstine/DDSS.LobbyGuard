using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_SteamLobby
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.GetRandomLobbyCode))]
        private static bool GetRandomLobbyCode_Prefix(ref string __result)
        {
            // Check if the setting is on
            if (!ConfigHandler.General.ExtendedInviteCodes.Value)
                return true;

            // Generate New Code
            __result = InviteCodeSecurity.GenerateNewCode();

            // Prevent Original
            return false;
        }
    }
}
