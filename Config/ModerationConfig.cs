using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class ModerationConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> PersistentBlacklist;

        //internal MelonPreferences_Entry<bool> PromptForKick;
        //internal MelonPreferences_Entry<bool> PromptForBlacklist;

        internal override string GetName()
            => "Moderation";

        internal override void CreatePreferences()
        {
            PersistentBlacklist = CreatePref("PersistentBlacklist",
                "Persistent Blacklist",
                "Makes Blacklisting of Players Persist and Save to File",
                true);
        }
    }
}
