using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_VirusController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.PerformPotentialVirusActivity))]
        private static bool PerformPotentialVirusActivity_Prefix(VirusController __instance)
        {
            // Validate Server
            if (!__instance.isServer)
                return true;

            // Validate Workstation
            if ((__instance.computerController == null)
                || (__instance.computerController.user == null))
                return false;

            // Validate Role
            if ((__instance.computerController.user.playerRole == PlayerRole.Slacker)
                || !__instance.isFirewallActive)
            {
                // Get Game Rule
                float probability = GameManager.instance.virusProbability;

                // RandomGen
                if (UnityEngine.Random.Range(0f, 100f) < (probability * 100f))
                    __instance.ServerSetVirus(true);
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VirusController), nameof(VirusController.InvokeUserCode_CmdSetFireWall__Boolean))]
        private static bool InvokeUserCode_CmdSetFireWall__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get VirusController
            VirusController controller = __0.TryCast<VirusController>();
            if ((controller == null)
                || (controller.computerController == null)
                || (controller.computerController._workStationController == null)
                || (controller.computerController._workStationController.usingPlayer == null))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, controller.transform.position))
                return false;

            // Check if Actually Sitting in Seat
            if (controller.computerController._workStationController.usingPlayer != sender)
                return false;

            // Get Value
            bool state = __1.ReadBool();
            if (controller.isFirewallActive == state)
                return false;

            // Run Game Command
            controller.UserCode_CmdSetFireWall__Boolean(state);

            // Prevent Original
            return false;
        }
    }
}
