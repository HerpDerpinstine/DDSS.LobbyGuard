using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class ModerationConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> PersistentBlacklist;
        internal MelonPreferences_Entry<bool> StickyNotesOnOpenDoors;

        //internal MelonPreferences_Entry<bool> PromptForKick;
        //internal MelonPreferences_Entry<bool> PromptForBlacklist;

        internal override MelonPreferences_Category CreateCategory(string filePath)
        {
            var cat = MelonPreferences.CreateCategory("Moderation", "Moderation");
            cat.SetFilePath(filePath, true, false);
            return cat;
        }

        internal override void CreatePreferences()
        {
            PersistentBlacklist = CreatePref("PersistentBlacklist",
                "Persistent Blacklist",
                "Makes Blacklisting of Players Persist and Save to File",
                true);

            StickyNotesOnOpenDoors = CreatePref("StickyNotesOnOpenDoors",
                "Sticky Notes on Open Doors",
                "Allows the Grabbing and Placing of Sticky Notes on Doors while they are Open",
                false);
        }
    }
}
