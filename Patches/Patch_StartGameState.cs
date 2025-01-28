using HarmonyLib;
using Il2CppGameManagement.StateMachine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_StartGameState
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartGameState), nameof(StartGameState.Enter))]
        private static bool Enter_Prefix(StartGameState __instance)
        {
            // Prevent Original
            return false;
        }
    }
}
