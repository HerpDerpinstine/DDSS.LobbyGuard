using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
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

            // Get Phone
            PhoneController phone = __0.TryCast<PhoneController>();

            NetworkIdentity sender = __2.identity;

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

            // Enforce Caller Number
            string caller = PhoneManager.instance.GetPhoneNumber(lobbyPlayer);

            // Get New Receiver
            __1.SafeReadNetworkIdentity();
            string receiver = __1.SafeReadString();

            // Run Security
            PhoneSecurity.OnCallAttempt(PhoneManager.instance, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
