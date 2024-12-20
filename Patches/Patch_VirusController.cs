using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
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
            if (InteractionSecurity.IsSlacker(__instance.computerController.user)
                || !__instance.NetworkisFirewallActive)
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
            if (controller == null)
                return false;

            // Validate Player
            if (sender.IsGhost()
                || !ComputerSecurity.ValidatePlayer(controller.computerController, sender))
                return false;

            // Get Value
            bool state = __1.SafeReadBool();
            if (controller.NetworkisFirewallActive == state)
                return false;

            // Run Game Command
            controller.UserCode_CmdSetFireWall__Boolean(state);

            // Prevent Original
            return false;
        }
    }
}
