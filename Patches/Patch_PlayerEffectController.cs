using HarmonyLib;
using Il2CppMirror;
using Il2CppPlayer.PlayerEffects;
using System;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_PlayerEffectController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerEffectController), nameof(PlayerEffectController.InvokeUserCode_CmdSetEffect__Int32__Single))]
        private static bool InvokeUserCode_CmdSetEffect__Int32__Single_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get PlayerEffectController
            PlayerEffectController controller = sender.GetComponent<PlayerEffectController>();
            if (controller == null)
                return false;

            // Get Effect Info
            int index = __1.ReadInt();
            float duration = __1.ReadFloat();

            // Validate Index
            if ((index < 0)
                || (index > (Enum.GetNames<PlayerEffects>().Length - 1)))
                return false;

            // Validate Duration
            if ((duration <= 0)
                || (duration > 180))
                return false;

            // Run Game Command
            controller.UserCode_CmdSetEffect__Int32__Single(index, duration);

            // Prevent Original
            return false;
        }
    }
}
