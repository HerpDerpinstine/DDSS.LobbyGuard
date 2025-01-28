using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Interactable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Interactable), nameof(Interactable.InvokeUserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient_Prefix(NetworkBehaviour __0,
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
            float distance = Vector3.Distance(sender.transform.position, interact.transform.position);
            if (distance < 0f)
                distance *= -1f;
            if (distance > InteractionSecurity.MAX_DISTANCE_CCTV)
                return false;

            // Validate Cooldown
            float coolDown = __1.ReadFloat();
            bool initial = __1.ReadBool();
            if (initial)
            {
                if (distance > InteractionSecurity.MAX_DISTANCE_DEFAULT)
                    return false;

                if ((coolDown <= 0f)
                    || (coolDown > InteractionSecurity.MAX_INTERACTION_COOLDOWN))
                    coolDown = InteractionSecurity.MAX_INTERACTION_COOLDOWN;
            }
            else
            {
                if ((distance <= InteractionSecurity.MAX_DISTANCE_DEFAULT)
                    || (interact.interactionTimeCounter <= 0f))
                    return false;

                if ((coolDown < 0f)
                    || (coolDown > 0f))
                    coolDown = 0f;
            }

            // Run Game Command
            interact.UserCode_CmdSetInteractionTimeCounter__NetworkIdentity__Single__Boolean__NetworkConnectionToClient(sender, coolDown, initial, __2);

            // Prevent Original
            return false;
        }
    }
}
