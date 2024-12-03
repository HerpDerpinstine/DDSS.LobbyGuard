using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;
using UnityEngine;

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

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, false))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cabinet.transform.position))
                return false;

            // Get Index
            __1.SafeReadNetworkIdentity();
            int itemIndex = __1.SafeReadInt();

            // Validate Index
            if ((itemIndex < 0)
                || (itemIndex > (cabinet.items.Count - 1)))
                return false;

            // Get Requested Collectible
            Collectible collectible = cabinet.items[itemIndex];
            if (collectible == null)
                return false;

            // Get Collectible Interactable Name
            string interactableName = collectible.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                cabinet.maxItemCount))
                return false;

            // Spawn a new Collectible
            GameObject gameObject = GameObject.Instantiate(collectible.gameObject, cabinet.transform.position, cabinet.transform.rotation);
            NetworkServer.Spawn(gameObject, __2);
            collectible = gameObject.GetComponent<Collectible>();
            collectible.UserCode_CmdUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Get StickyNoteController
            eConfigHostType configValue = ConfigHandler.Gameplay.UsernamesOnStickyNotes.Value;
            if ((configValue != eConfigHostType.DISABLED)
                && (collectible.GetIl2CppType() == Il2CppType.Of<StickyNoteController>()))
            {
                // Apply New Name
                string userName = sender.GetUserName();
                if (!string.IsNullOrEmpty(userName)
                    && !string.IsNullOrWhiteSpace(userName))
                {
                    string newName = $"{userName.RemoveRichText()}'s Sticky Note";
                    if (configValue == eConfigHostType.HOST_ONLY)
                        collectible.interactableName =
                            collectible.label = newName;
                    else
                        collectible.NetworkinteractableName =
                            collectible.Networklabel = newName;
                }
            }

            // Prevent Original
            return false;
        }
    }
}
