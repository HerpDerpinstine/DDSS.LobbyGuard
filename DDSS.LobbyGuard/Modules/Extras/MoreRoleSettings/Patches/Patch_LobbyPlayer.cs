using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_LobbyPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Prefix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.DeserializeSyncVars))]
        private static void DeserializeSyncVars_Postfix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Prefix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LobbyPlayer), nameof(LobbyPlayer.SerializeSyncVars))]
        private static void SerializeSyncVars_Postfix(LobbyPlayer __instance)
            => EnforcePlayerValues(__instance);

        private static void EnforcePlayerValues(LobbyPlayer __instance)
        {
            if (!NetworkServer.activeHost
                || (GameManager.instance == null)
                || GameManager.instance.WasCollected)
                return;

            // Check if Win Screen is Hidden
            if (GameManager.instance.GetWinner() == PlayerRole.None)
            {
                // Force NetworkisFired to false for Janitors to be Reassignable
                if (ModuleConfig.Instance.AllowJanitorsToKeepWorkStation.Value
                    && __instance.IsJanitor())
                    __instance.isFired = false;
            }
        }
    }
}
