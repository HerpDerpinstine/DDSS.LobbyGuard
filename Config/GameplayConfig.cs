using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GameplayConfig : ConfigCategory
    {
        //internal MelonPreferences_Entry<eConfigHostType> UsernamesOnPrintedDocuments;
        //internal MelonPreferences_Entry<eConfigHostType> UsernamesOnPrintedImages;
        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnStickyNotes;

        internal MelonPreferences_Entry<bool> StickyNotesOnPlayers;
        internal MelonPreferences_Entry<bool> StickyNotesOnOpenDoors;
        internal MelonPreferences_Entry<bool> StickyNotesOnClosedDoors;

        internal override string GetName()
            => "Gameplay";

        internal override void CreatePreferences()
        {
            //UsernamesOnPrintedDocuments = CreatePref("UsernamesOnPrintedDocuments",
            //    "Usernames On Printed Documents",
            //    "Puts the Player's Username in the Name of their Custom Printed Document",
            //    eConfigHostType.ALL);

            //UsernamesOnPrintedImages = CreatePref("UsernamesOnPrintedImages",
            //    "Usernames On Printed Images",
            //    "Puts the Player's Username in the Name of their Custom Printed Image",
            //    eConfigHostType.ALL);

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
        }
    }
}
