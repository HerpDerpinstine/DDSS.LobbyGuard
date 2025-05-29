using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.StickyNote
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnStickyNotes;
        internal MelonPreferences_Entry<bool> StickyNotesOnPlayers;
        internal MelonPreferences_Entry<bool> StickyNotesOnDoors;

        public ModuleConfig() : base()
            => Instance = this;
        public override eConfigType ConfigType
            => eConfigType.Security;
        public override string ID
            => "StickyNote";
        public override string DisplayName
            => "Sticky Note";

        public override void CreatePreferences()
        {
            UsernamesOnStickyNotes = CreatePref("UsernamesOnStickyNotes",
                "Usernames On Sticky Notes",
                "Puts the Player's Username in the Name of their Custom Sticky Note   [ ALL | HOST_ONLY | DISABLED ]",
                eConfigHostType.HOST_ONLY);

            StickyNotesOnPlayers = CreatePref("StickyNotesOnPlayers",
                "Sticky Notes on Players",
                "Allows the Grabbing and Placing of Sticky Notes on Players",
                true);

            StickyNotesOnDoors = CreatePref("StickyNotesOnDoors",
                "Sticky Notes on Doors",
                "Allows the Grabbing and Placing of Sticky Notes on Doors",
                true);
        }
    }
}
