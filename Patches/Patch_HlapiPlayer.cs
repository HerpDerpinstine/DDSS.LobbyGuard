using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppDissonance.Integrations.UNet_HLAPI;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_HlapiPlayer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(HlapiPlayer), nameof(HlapiPlayer.InvokeUserCode_CmdSetPlayerName__String))]
        private static bool InvokeUserCode_CmdSetPlayerName__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1)
        {
            // Get HlapiPlayer
            HlapiPlayer player = __0.TryCast<HlapiPlayer>();
            if ((player == null)
                || player.WasCollected
                || (!string.IsNullOrEmpty(player.Network_playerId) && !string.IsNullOrWhiteSpace(player.Network_playerId)))
                return false;

            string newName = __1.SafeReadString();
            if (string.IsNullOrEmpty(newName)
                || string.IsNullOrWhiteSpace(newName)) 
                return false;

            player.UserCode_CmdSetPlayerName__String(newName);

            // Prevent Original
            return false;
        }
    }
}