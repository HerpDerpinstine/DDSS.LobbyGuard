using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
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

            // Enforce Receiver Number
            string receiver = phone.phoneNumber;
            if (string.IsNullOrEmpty(receiver)
                || string.IsNullOrWhiteSpace(receiver))
                return false;

            // Run Security
            PhoneSecurity.OnCallAnswer(PhoneManager.instance, caller, receiver);

            // Prevent Original
            return false;
        }
    }
}
