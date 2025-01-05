﻿
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Interactable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Interactable), nameof(Interactable.InvokeUserCode_CmdSetInteractionCoolDown__Single))]
        private static bool InvokeUserCode_CmdSetInteractionCoolDown__Single_Prefix(NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Interactable
            WhiteBoardController interact = __0.TryCast<WhiteBoardController>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position, InteractionSecurity.MAX_DISTANCE_CCTV))
                return false;

            // Validate Cooldown
            float coolDown = __1.SafeReadFloat();
            if (coolDown != GameManager.instance.NetworkmeetingCooldown)
                return false;

            // Run Game Command
            interact.UserCode_CmdSetInteractionCoolDown__Single(coolDown);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Interactable), nameof(Interactable.InvokeUserCode_CmdSetInteractionTimeCounter__Single__Boolean))]
        private static bool InvokeUserCode_CmdSetInteractionTimeCounter__Single__Boolean_Prefix(NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Interactable
            Interactable interact = __0.TryCast<Interactable>();
            if ((interact == null)
                || interact.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, interact.transform.position, InteractionSecurity.MAX_DISTANCE_CCTV))
                return false;

            // Validate Cooldown
            float coolDown = __1.ReadFloat();
            bool initial = __1.ReadBool();
            if (initial)
            {
                if ((coolDown <= 0f)
                    || (coolDown > 600f))
                    coolDown = 600f;
            }
            else
            {
                if ((coolDown < 0f)
                    || (coolDown > 0f))
                    coolDown = 0f;
            }

            // Run Game Command
            interact.UserCode_CmdSetInteractionTimeCounter__Single__Boolean(coolDown, initial);

            // Prevent Original
            return false;
        }
    }
}
