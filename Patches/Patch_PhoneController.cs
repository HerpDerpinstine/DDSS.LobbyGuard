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

            // Get Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            NetworkIdentity chairIdentity = controller.NetworkcurrentChair;
            if ((chairIdentity == null)
                || chairIdentity.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = chairIdentity.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected
                || (workStation.NetworkownerLobbyPlayer == null)
                || workStation.NetworkownerLobbyPlayer.WasCollected)
                return false;

            lobbyPlayer = workStation.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Enforce Receiver Number
            string caller = phone.phoneNumber;
            if (string.IsNullOrEmpty(caller)
                || string.IsNullOrWhiteSpace(caller))
                return false;

            // Get New Caller
            __1.SafeReadString();
            string receiver = __1.SafeReadString();
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Run Security
            PhoneSecurity.OnCallAttempt(PhoneManager.instance, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
