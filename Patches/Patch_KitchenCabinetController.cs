using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_KitchenCabinetController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(KitchenCabinetController), nameof(KitchenCabinetController.InvokeUserCode_CmdSpawnAndGrab__NetworkIdentity__Int32__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSpawnAndGrab__NetworkIdentity__Int32__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get KitchenCabinetController
            KitchenCabinetController cabinet = __0.TryCast<KitchenCabinetController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cabinet.transform.position))
                return false;

            // Get Index
            __1.ReadNetworkIdentity();
            int itemIndex = __1.ReadInt();

            // Validate Index
            if ((itemIndex < 0)
                || (itemIndex > (cabinet.items.Count - 1)))
                return false;

            // Get Requested Item
            Collectible item = cabinet.items[itemIndex];
            if (item == null)
                return false;

            // Get Item Interactable Name
            string interactableName = item.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                cabinet.maxItemCount))
                return false;

            // Run Game Command
            cabinet.UserCode_CmdSpawnAndGrab__NetworkIdentity__Int32__NetworkConnectionToClient(sender, itemIndex, __2);

            // Prevent Original
            return false;
        }
    }
}
