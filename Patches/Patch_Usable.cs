﻿using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Scripts;
using Il2CppPlayer.StateMachineLogic;
using Il2CppProps.Door;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Usable
    {
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

            // Validate Distance
            if (!__2.identity.isServer
                && !InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
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
                            if (!ConfigHandler.Gameplay.StickyNotesOnOpenDoors.Value
                                && (door.Networkstate != 0))
                                return false;
                            if (!ConfigHandler.Gameplay.StickyNotesOnClosedDoors.Value
                                && (door.Networkstate == 0))
                                return false;
                        }

                        // Get PlayerInteractable
                        PlayerInteractable playerInteractable = holder.parentInteractable.TryCast<PlayerInteractable>();
                        if ((playerInteractable != null)
                            && !playerInteractable.WasCollected)
                        {
                            // Validate Player
                            if (!ConfigHandler.Gameplay.StickyNotesOnPlayers.Value)
                                return false;
                        }
                    }
                }
            }
            
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller != null)
                && !controller.WasCollected)
            {
                // Check for Handshake
                if (!ConfigHandler.Gameplay.GrabbingWhileHandshaking.Value
                    && (controller.NetworktargetUBState == (int)UpperBodyStates.RequestHandShake)
                    && (controller.NetworktargetUBState == (int)UpperBodyStates.PerformHandShake))
                    return false;

                // Check for Emotes
                if (!ConfigHandler.Gameplay.GrabbingWhileEmoting.Value
                    && ((controller.NetworktargetState == (int)States.Dancing)
                        || (controller.NetworktargetState == (int)States.Humping)
                        || (controller.NetworktargetState == (int)States.Beg)
                        || (controller.NetworktargetUBState == (int)UpperBodyStates.PoundChest)
                        || (controller.NetworktargetUBState == (int)UpperBodyStates.Waving)
                        || (controller.NetworktargetUBState == (int)UpperBodyStates.Clapping)
                        || (controller.NetworktargetUBState == (int)UpperBodyStates.FacePalm)))
                    return false;
            }

            // Run Game Command
            usable.UserCode_CmdUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdUseNoTypeVerification__NetworkIdentity))]
        private static bool InvokeUserCode_CmdUseNoTypeVerification__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Usable
            Usable usable = __0.TryCast<Usable>();
            if ((usable == null)
                || usable.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
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
                            if (!ConfigHandler.Gameplay.StickyNotesOnOpenDoors.Value
                                && (door.Networkstate != 0))
                                return false;
                            if (!ConfigHandler.Gameplay.StickyNotesOnClosedDoors.Value
                                && (door.Networkstate == 0))
                                return false;
                        }

                        // Get PlayerInteractable
                        PlayerInteractable playerInteractable = holder.parentInteractable.TryCast<PlayerInteractable>();
                        if ((playerInteractable != null)
                            && !playerInteractable.WasCollected)
                        {
                            // Validate Player
                            if (!ConfigHandler.Gameplay.StickyNotesOnPlayers.Value)
                                return false;
                        }
                    }
                }
            }

            // Run Game Command
            usable.UserCode_CmdUseNoTypeVerification__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Usable), nameof(Usable.InvokeUserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Usable
            Usable usable = __0.TryCast<Usable>();
            if ((usable == null)
                || usable.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, usable.transform.position))
                return false;

            // Run Game Command
            usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
