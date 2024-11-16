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

        internal MelonPreferences_Entry<bool> ProductivityFromTaskCompletion;

        internal MelonPreferences_Entry<bool> PlayerVelocityEnforcement;
        internal MelonPreferences_Entry<bool> HideSlackersFromClients;

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

            PlayerVelocityEnforcement = CreatePref("PlayerVelocityEnforcement",
                "Player Velocity Enforcement",
                "Prevents Speedhacking using Context-Based Velocity Clamping",
                true);

            HideSlackersFromClients = CreatePref("HideSlackersFromClients",
                "Hide Slackers from Clients",
                "Prevents Slacker Count from being broadcasted to Clients",
                false);
        }
    }
}
