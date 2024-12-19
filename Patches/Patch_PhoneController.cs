using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.WorkStation.Phone;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PhoneController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PhoneController), nameof(PhoneController.InvokeUserCode_CmdCall__NetworkIdentity__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCall__NetworkIdentity__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get PhoneController
            PhoneController phone = __0.TryCast<PhoneController>();
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get New Caller
            string caller = __1.SafeReadString();
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get and Ignore User Input Receiver
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Check for Server
            if (!__2.identity.isServer)
            {
                // Get Player
                LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected
                    || phone.workStationController.usingPlayer != __2.identity)
                    return false;

                // Validate Sender is on Workstation
                PlayerController controller = __2.identity.GetComponent<PlayerController>();
                if ((controller == null)
                    || controller.WasCollected
                    || (controller.NetworkcurrentChair == null)
                    || controller.NetworkcurrentChair.WasCollected)
                    return false;

                // Validate Chair
                WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
                if ((workStation == null)
                    || workStation.WasCollected)
                    return false;

                // Enforce Receiver Number
                receiver = phone.phoneNumber;
                if (string.IsNullOrEmpty(receiver)
                    || string.IsNullOrWhiteSpace(receiver))
                    return false;
            }

            // Run Security
            PhoneSecurity.OnCallAnswer(PhoneManager.instance, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
