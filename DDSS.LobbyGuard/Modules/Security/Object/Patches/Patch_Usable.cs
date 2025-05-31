using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Scripts;
using Il2CppProps.Door;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Usable
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Usable
            Usable usable = __0.TryCast<Usable>();
            if ((usable == null)
                || usable.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Validate Distance
            if (controller.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Validate Drop
            if (!InteractionSecurity.CanStopUseUsable(sender, usable.TryCast<Chair>()))
                return false;

            // Validate Chair
            /*
            WorkStationController workStation = usable.TryCast<WorkStationController>();
            if ((workStation != null)
                && !workStation.WasCollected)
            {
                PhoneController phone = workStation.phoneController;
                if ((phone != null)
                    && !phone.WasCollected)
                    phone.ForceCallToEnd(sender, __2);
            }
            */

            // Run Game Command
            usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdUse__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdUse__NetworkIdentity__NetworkConnectionToClient_Prefix(
             NetworkBehaviour __0,
             NetworkConnectionToClient __2)
        {
            // Get Usable
            Usable usable = __0.TryCast<Usable>();
            if ((usable == null)
                || usable.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            // Validate Distance
            if (controller.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, usable))
                return false;

            // Get Usable Type
            Il2CppSystem.Type usableType = usable.GetIl2CppType();
            if ((usableType == null)
                || usableType.WasCollected)
                return false;

            // Check for StickyNoteController
            if (usableType == Il2CppType.Of<StickyNoteController>())
            {
                // Get StickyNoteController
                StickyNoteController stickyNote = usable.TryCast<StickyNoteController>();
                if ((stickyNote != null)
                    && !stickyNote.WasCollected)
                {
                    // Get Holder
                    CollectibleHolder holder = stickyNote.currentHolder;
                    if ((holder != null)
                        && !holder.WasCollected)
                    {
                        // Get DoorInteractable
                        DoorInteractable doorInteractable = holder.parentInteractable.TryCast<DoorInteractable>();
                        if ((doorInteractable != null)
                            && !doorInteractable.WasCollected)
                        {
                            // Get DoorController
                            DoorController door = doorInteractable.doorController;
                            if ((door == null)
                                || door.WasCollected)
                                return false;

                            // Validate Door State
                            if (!StickyNote.ModuleConfig.Instance.StickyNotesOnDoors.Value)
                                return false;
                        }

                        // Get PlayerInteractable
                        PlayerInteractable playerInteractable = holder.parentInteractable.TryCast<PlayerInteractable>();
                        if ((playerInteractable != null)
                            && !playerInteractable.WasCollected)
                        {
                            // Validate Player
                            if (!StickyNote.ModuleConfig.Instance.StickyNotesOnPlayers.Value)
                                return false;
                        }
                    }
                }
            }

            // Run Game Command
            usable.UserCode_CmdUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}