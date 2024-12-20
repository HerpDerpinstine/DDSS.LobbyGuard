using MelonLoader;

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

        internal MelonPreferences_Entry<bool> PrinterCopiesWithSignature;

        internal MelonPreferences_Entry<bool> CloseDoorsOnLock;

        internal MelonPreferences_Entry<bool> SpawnManagerKeys;

        internal MelonPreferences_Entry<bool> AllowJanitorsToVote;

        internal MelonPreferences_Entry<bool> ProductivityFromTaskCompletion;

        internal MelonPreferences_Entry<bool> PlayerLeavesReduceTerminations;

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

            PrinterCopiesWithSignature = CreatePref("PrinterCopiesWithSignature",
                "Printer Copies with Signature",
                "Printer transfers the Signature of a Signed Document over to the Copy Document",
                false);

            CloseDoorsOnLock = CreatePref("CloseDoorsOnLock",
                "Close Doors On Lock",
                "Close Doors when Locking",
                true);

            SpawnManagerKeys = CreatePref("SpawnManagerKeys",
                "Spawn Manager Keys",
                "Allows Spawning of Keys in Manager's Office",
                true);

            AllowJanitorsToVote = CreatePref("AllowJanitorsToVote",
                "Allow Janitors to Vote",
                "Allows Janitors to Vote on Manager",
                false);

            ProductivityFromTaskCompletion = CreatePref("ProductivityFromTaskCompletion",
                "Productivity from Task Completion",
                "Allows Productivity to be Gained from Task Completion",
                true);

            PlayerLeavesReduceTerminations = CreatePref("PlayerLeavesReduceTerminations",
                "Player Leaves Reduce Terminations",
                "Allows Terminations to be Decreased from Players Disconnecting",
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
