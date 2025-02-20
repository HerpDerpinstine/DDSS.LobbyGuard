using HarmonyLib;
using Il2Cpp;

namespace DDSS_LobbyGuard.Modules.Extras.ExtendedInviteCodes.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_SteamLobby
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.GetRandomLobbyCode))]
        private static bool GetRandomLobbyCode_Prefix(ref string __result)
        {
            // Generate New Code
            __result = ModuleMain.GenerateNewCode();

            // Prevent Original
            return false;
        }
    }
}