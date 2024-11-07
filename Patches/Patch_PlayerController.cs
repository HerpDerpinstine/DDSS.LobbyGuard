using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using Il2CppPlayer;
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
        private static bool InvokeUserCode_CmdMovePlayer__Vector3_Prefix(NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            if (!__2.identity.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.UserCode_CmdMovePlayer__Vector3))]
        private static void UserCode_CmdMovePlayer__Vector3_Prefix(PlayerController __instance, Vector3 __0)
        {
            __instance.lastPos = __0;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.InvokeUserCode_CmdSpank__NetworkIdentity))]
        private static bool InvokeUserCode_CmdSpank__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get PlayerController
            PlayerController player = __0.TryCast<PlayerController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position, 
                player.transform.position, 
                InteractionSecurity.MAX_SPANK_DISTANCE))
                return false;

            // Run Game Command
            player.UserCode_CmdSpank__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerController), nameof(PlayerController.SetLocalVelocity))]
        private static void SetLocalVelocity_Prefix(PlayerController __instance)
        {
            // Check for Game Start
            if ((GameManager.instance.NetworktargetGameState != (int)GameStates.InGame)
                && (GameManager.instance.NetworktargetGameState != (int)GameStates.Meeting))
                return;

            // Calculate First Velocity
            Vector3 oldPos = __instance.lastPos;
            Vector3 currentPos = __instance.transform.position;
            var heading = (currentPos - oldPos);
            heading.y = 0;
            var currentSpeed = heading.magnitude;

            // Check Sprint
            bool isSprinting = __instance.NetworktargetState == (int)States.Sprint;
            bool hasCoffeeEffect = !GameManager.instance.noSprinting 
                || (__instance.GetComponent<PlayerEffectController>().NetworktargetState == (int)PlayerEffects.SpeedBoost);

            // Get Max Speed
            float maxSpeed = ((isSprinting && hasCoffeeEffect) 
                ? __instance.sprintSpeed
                : __instance.moveSpeed) / 100f;

            // Validate Speed
            if (currentSpeed <= (maxSpeed + 0.015f))
                return;

            // Set New Position
            __instance.transform.position = oldPos;
            __instance.CmdMovePlayer(oldPos);
        }
    }
}
