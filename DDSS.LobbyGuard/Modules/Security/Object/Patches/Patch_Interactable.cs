﻿using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
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

            // Get Distance
            float maxDistance = InteractionSecurity.MAX_INTERACTION_DISTANCE;
            float distance = Vector3.Distance(sender.transform.position, interact.transform.position);
            if (distance < 0f)
                distance *= -1f;
            if (distance > (maxDistance + 1f))
                return false;

            // Validate Cooldown
            __1.SafeReadNetworkIdentity();
            float coolDown = __1.ReadFloat();
            bool initial = __1.ReadBool();
            if (initial)
            {
                if (distance > maxDistance)
                    return false;

                if ((coolDown <= 0f)
                    || (coolDown > InteractionSecurity.MAX_INTERACTION_COOLDOWN))
                    coolDown = InteractionSecurity.MAX_INTERACTION_COOLDOWN;
            }
            else
            {
                if ((distance <= maxDistance)
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