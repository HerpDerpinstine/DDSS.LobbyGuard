﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.PlayerEffects;
using Il2CppPlayer.StateMachineLogic;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdMovePlayer__Vector3))]
        private static bool InvokeUserCode_CmdMovePlayer__Vector3_Prefix(NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Run Game Command
            controller.UserCode_CmdMovePlayer__Vector3(__1.ReadVector3());

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdSpank__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSpank__NetworkIdentity_Prefix(NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Run Game Command
            controller.UserCode_CmdSpank__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.SetLocalVelocity))]
        private static void SetLocalVelocity_Prefix(PlayerController __instance)
        {
            if (!ConfigHandler.Gameplay.PlayerVelocityEnforcement.Value)
                return;

            // Check for Chair
            if (!NetworkServer.activeHost
                || !__instance.isServer
                || __instance.isInChair)
                return;

            // Check for Game Start
            if ((GameManager.instance.NetworktargetGameState != (int)GameStates.InGame)
                && (GameManager.instance.NetworktargetGameState != (int)GameStates.Meeting))
                return;

            // Check Sprint
            bool isSprinting = __instance.NetworktargetState == (int)States.Sprint;
            bool hasCoffeeEffect = !GameManager.instance.noSprinting
                || (__instance.GetComponent<PlayerEffectController>().NetworktargetState == (int)PlayerEffects.SpeedBoost);

            // Get Max Speed
            float maxSpeed = ((isSprinting && hasCoffeeEffect)
                ? __instance.sprintSpeed
                : __instance.moveSpeed) * Time.unscaledDeltaTime;

            // Validate Speed
            Vector3 velocity = (__instance.transform.position - __instance.lastPos);
            Vector3 velocityNoUp = velocity;
            velocityNoUp.y = 0f;
            float magnitude = velocityNoUp.magnitude;
            if (magnitude <= maxSpeed)
                return;

            // Clamp Speed
            Vector3 newDistance = Vector3.ClampMagnitude(velocityNoUp, maxSpeed);
            newDistance.y = velocity.y;

            // Force Position to Location
            Vector3 correctedPos = __instance.lastPos + newDistance;
            ForcePosition(__instance, correctedPos);
        }

        private static void ForcePosition(PlayerController __instance, Vector3 pos)
        {
            __instance.enabled = false;
            __instance.netIdentity.enabled = false;

            if (__instance.isLocalPlayer)
                __instance.controller.enabled = false;

            NetworkTransform trans = __instance.gameObject.GetComponent<NetworkTransform>();
            if (trans != null)
                trans.enabled = false;

            __instance.transform.position = pos;
            __instance.SetDirty();

            if (trans != null)
            {
                trans.UserCode_CmdTeleport__Vector3(pos);
                trans.SetPosition(pos);
                trans.SetDirty();
                trans.enabled = true;
            }

            if (__instance.isLocalPlayer)
                __instance.controller.enabled = true;

            __instance.netIdentity.enabled = true;
            __instance.enabled = true;

            if (__instance.NetworktargetState == (int)States.Idle)
                __instance.NetworktargetState = (int)States.Movement;
        }
    }
}
