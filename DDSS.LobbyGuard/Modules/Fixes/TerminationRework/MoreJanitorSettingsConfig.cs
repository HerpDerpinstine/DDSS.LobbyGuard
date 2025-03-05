using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Fixes.TerminationRework
{
    internal class MoreJanitorSettingsConfig : ConfigCategory
    {
        internal static MoreJanitorSettingsConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> AllowJanitorsToKeepWorkStation;

        public MoreJanitorSettingsConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreJanitorSettings";
        public override string GetDisplayName()
            => "More Janitor Settings";

        public override void CreatePreferences()
        {
            AllowJanitorsToKeepWorkStation = CreatePref("AllowJanitorsToKeepWorkStation",
                "Allow Janitors to Keep WorkStation",
                "Allows Janitors to Keep their Assigned WorkStation",
                false);
        }
    }
}
