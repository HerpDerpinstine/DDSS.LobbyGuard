using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Extras.ExtendedInviteCodes.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_NetworkManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkManager), nameof(NetworkManager.StartHost))]
        private static void StartHost_Prefix()
        {
            if (!ModuleConfig.Instance.ExtendedInviteCodes.Value)
                return;

            // Check if already has Invite Code
            if (!string.IsNullOrEmpty(SteamLobby.requestedLobbyCode)
                && !string.IsNullOrWhiteSpace(SteamLobby.requestedLobbyCode))
                return;

            // Generate New Code
            SteamLobby.requestedLobbyCode = ModuleMain.GenerateNewCode();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(SteamLobby), nameof(SteamLobby.GetRandomLobbyCode))]
        private static bool GetRandomLobbyCode_Prefix(ref string __result)
        {
            if (!ModuleConfig.Instance.ExtendedInviteCodes.Value)
                return true;

            // Generate New Code
            __result = ModuleMain.GenerateNewCode();

            // Prevent Original
            return false;
        }
    }
}