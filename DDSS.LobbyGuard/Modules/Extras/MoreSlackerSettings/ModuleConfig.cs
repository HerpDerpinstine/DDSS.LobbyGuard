using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreSlackerSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> SlackerTrashBinFireDelay;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreSlackerSettings";
        public override string GetDisplayName()
            => "More Slacker Settings";

        public override void CreatePreferences()
        {
            SlackerTrashBinFireDelay = CreatePref("SlackerTrashBinFireDelay",
                 "Slacker TrashBin Fire Delay",
                 "Seconds until TrashBin Fire Ignites from Slacker Task",
                 4);
        }
    }
}
