using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.Scripts;
using Il2CppProps.VendingMachine;

namespace DDSS_LobbyGuard.Modules.Security.Shelf.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_VendingMachine
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(VendingMachine), nameof(VendingMachine.InvokeUserCode_CmdPlayAudioClip__NetworkIdentity__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPlayAudioClip__NetworkIdentity__String__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get VendingMachine
            VendingMachine machine = __0.TryCast<VendingMachine>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, machine))
                return false;

            // Get Interactible Name
            __1.SafeReadNetworkIdentity();
            string interactibleName = __1.SafeReadString();
            if (string.IsNullOrEmpty(interactibleName)
                || string.IsNullOrWhiteSpace(interactibleName))
                return false;

            // Run Game Command
            machine.UserCode_CmdPlayAudioClip__NetworkIdentity__String__NetworkConnectionToClient(sender, interactibleName, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(VendingMachine), nameof(VendingMachine.InvokeUserCode_CmdSpawnItem__NetworkIdentity__Int32__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSpawnItem__NetworkIdentity__Int32__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get VendingMachine
            VendingMachine machine = __0.TryCast<VendingMachine>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, machine))
                return false;

            // Get Index
            __1.SafeReadNetworkIdentity();
            int itemIndex = __1.ReadByte() / 2;

            // Validate Index
            if ((itemIndex < 0)
                || (itemIndex > (machine.items.Count - 1)))
                return false;

            // Get Requested Item
            Collectible item = machine.items[itemIndex];
            if (item == null)
                return false;

            // Get Item Interactable Name
            string interactableName = item.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                machine.maxItemCount))
                return false;

            // Run Game Command
            machine.UserCode_CmdSpawnItem__NetworkIdentity__Int32__NetworkConnectionToClient(sender, itemIndex, __2);

            // Prevent Original
            return false;
        }
    }
}