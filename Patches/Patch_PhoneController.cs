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
            NetworkIdentity sender = __2.identity;

            // Get Phone
            PhoneController phone = __0.TryCast<PhoneController>();
            if ((phone == null)
                || phone.WasCollected)
                return false;

            // Get Player
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chair = controller.NetworkcurrentChair;
            if ((chair == null)
                || chair.WasCollected)
                return false;

            WorkStationController station = chair.GetComponent<WorkStationController>();
            if ((station == null)
                || station.WasCollected
                || (station.phoneController != phone))
                return false;

            // Enforce Caller Number
            string caller = phone.phoneNumber;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get New Receiver
            __1.SafeReadNetworkIdentity();
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Run Security
            PhoneSecurity.OnCallAttempt(phone, caller, receiver, sender);

            // Prevent Original
            return false;
        }
    }
}
