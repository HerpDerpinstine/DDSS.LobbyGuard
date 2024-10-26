﻿using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class ModerationConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> PersistentBlacklist;
        //internal MelonPreferences_Entry<bool> PromptForKick;
        //internal MelonPreferences_Entry<bool> PromptForBlacklist;

        internal override MelonPreferences_Category CreateCategory(string filePath)
        {
            var cat = MelonPreferences.CreateCategory("Moderation", "Moderation");
            cat.SetFilePath(filePath, true);
            return cat;
        }

        internal override void CreatePreferences()
        {
            PersistentBlacklist = CreatePref("PersistentBlacklist",
                "Persistent Blacklist",
                "Makes Blacklisting of Players Persist and Save to File",
                true);
        }
    }
}
