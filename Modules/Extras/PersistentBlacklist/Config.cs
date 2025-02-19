using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.PersistentBlacklist
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> PersistentBlacklist;

        //internal MelonPreferences_Entry<bool> PromptForKick;
        //internal MelonPreferences_Entry<bool> PromptForBlacklist;

        public ModuleConfig() : base() 
            => Instance = this;
        public override string GetName()
            => "Moderation";

        public override void CreatePreferences()
        {
            PersistentBlacklist = CreatePref("PersistentBlacklist",
                "Persistent Blacklist",
                "Makes Blacklisting of Players Persist and Save to File",
                true);
        }
    }
}
