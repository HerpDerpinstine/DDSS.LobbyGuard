﻿using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GameplayConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> UsernamesOnPrintedDocuments;
        internal MelonPreferences_Entry<bool> UsernamesOnPrintedImages;
        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnStickyNotes;

        internal MelonPreferences_Entry<bool> StickyNotesOnPlayers;
        internal MelonPreferences_Entry<bool> StickyNotesOnOpenDoors;
        internal MelonPreferences_Entry<bool> StickyNotesOnClosedDoors;

        internal MelonPreferences_Entry<bool> ProductivityFromTaskCompletion;

        internal MelonPreferences_Entry<bool> PlayerVelocityEnforcement;
        internal MelonPreferences_Entry<bool> HideSlackersFromClients;

        internal MelonPreferences_Entry<bool> GrabbingWhileEmoting;
        internal MelonPreferences_Entry<bool> DroppingWhileEmoting;

        internal MelonPreferences_Entry<bool> GrabbingWhilePointing;
        internal MelonPreferences_Entry<bool> DroppingWhilePointing;

        internal MelonPreferences_Entry<bool> GrabbingWhileHandshaking;
        internal MelonPreferences_Entry<bool> DroppingWhileHandshaking;

        internal override string GetName()
            => "Gameplay";

        internal override void CreatePreferences()
        {
            UsernamesOnPrintedDocuments = CreatePref("UsernamesOnPrintedDocuments",
                "Usernames On Printed Documents",
                "Puts the Player's Username in the Name of their Custom Printed Document",
                true);

            UsernamesOnPrintedImages = CreatePref("UsernamesOnPrintedImages",
                "Usernames On Printed Images",
                "Puts the Player's Username in the Name of their Custom Printed Image",
                true);

            UsernamesOnStickyNotes = CreatePref("UsernamesOnStickyNotes",
                "Usernames On Sticky Notes",
                "Puts the Player's Username in the Name of their Custom Sticky Note   [ ALL | HOST_ONLY | DISABLED ]",
                eConfigHostType.HOST_ONLY);

            StickyNotesOnPlayers = CreatePref("StickyNotesOnPlayers",
                "Sticky Notes on Players",
                "Allows the Grabbing and Placing of Sticky Notes on Players",
                true);

            StickyNotesOnOpenDoors = CreatePref("StickyNotesOnOpenDoors",
                "Sticky Notes on Open Doors",
                "Allows the Grabbing and Placing of Sticky Notes on Doors while they are Open",
                false);

            StickyNotesOnClosedDoors = CreatePref("StickyNotesOnClosedDoors",
                "Sticky Notes on Closed Doors",
                "Allows the Grabbing and Placing of Sticky Notes on Doors while they are Closed",
                true);

            ProductivityFromTaskCompletion = CreatePref("ProductivityFromTaskCompletion",
                "Productivity from Task Completion",
                "Allows Productivity to be Gained from Task Completion",
                true);

            PlayerVelocityEnforcement = CreatePref("EnforcePlayerVelocity",
                "Enforce Player Velocity",
                "Prevents Speedhacking using Context-Based Velocity Clamping",
                false);

            HideSlackersFromClients = CreatePref("HideSlackersFromClients",
                "Hide Slackers from Clients",
                "Prevents Slacker Count from being broadcasted to Clients",
                false);

            GrabbingWhileEmoting = CreatePref("GrabbingWhileEmoting",
                "Grabbing while Emoting",
                "Allows the Grabbing of Usables while Emoting",
                false);

            DroppingWhileEmoting = CreatePref("DroppingWhileEmoting",
                "Dropping while Emoting",
                "Allows the Dropping of Usables while Emoting",
                false);

            GrabbingWhilePointing = CreatePref("GrabbingWhilePointing",
                "Grabbing while Pointing",
                "Allows the Grabbing of Usables while Pointing",
                false);

            DroppingWhilePointing = CreatePref("DroppingWhilePointing",
                "Dropping while Pointing",
                "Allows the Dropping of Usables while Pointing",
                false);

            GrabbingWhileHandshaking = CreatePref("GrabbingWhileHandshaking",
                "Grabbing while Handshaking",
                "Allows the Grabbing of Usables while Handshaking",
                false);

            DroppingWhileHandshaking = CreatePref("DroppingWhileHandshaking",
                "Dropping while Handshaking",
                "Allows the Dropping of Usables while Handshaking",
                false);
        }
    }
}
