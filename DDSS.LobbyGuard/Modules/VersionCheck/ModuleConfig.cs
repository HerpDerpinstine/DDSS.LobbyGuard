using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.VersionCheck
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> CheckForUpdates;
        internal MelonPreferences_Entry<bool> ShowPrompt;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.General;
        public override string GetName()
            => "VersionCheck";
        public override string GetDisplayName()
            => "Version Check";

        public override void CreatePreferences()
        {
            CheckForUpdates = CreatePref("CheckForUpdates",
               "Check For Updates",
               "Checks to see if there is an Update available",
               true);
            
            ShowPrompt = CreatePref("ShowPrompt",
               "Show Prompt",
               "Shows a Prompt if an Update is found",
               true);
        }
    }
}
