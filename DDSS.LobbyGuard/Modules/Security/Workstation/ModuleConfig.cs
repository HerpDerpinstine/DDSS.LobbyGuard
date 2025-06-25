using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Workstation
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> WorkstationVirusRandomMinimum;
        internal MelonPreferences_Entry<int> WorkstationVirusRandomMaximum;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Workstation";
        public override string DisplayName
            => "Workstation";

        public override void CreatePreferences()
        {
            WorkstationVirusRandomMinimum = CreatePref("WorkstationVirusRandomMinimum",
                "Workstation Virus Random Minimum",
                "Minimum Value in Seconds for the Random Timer of Workstation Viruses",
                30);

            WorkstationVirusRandomMaximum = CreatePref("WorkstationVirusRandomMaximum",
                "Workstation Virus Random Maximum",
                "Maximum Value in Seconds for the Random Timer of Workstation Viruses",
                60);
        }
    }
}
