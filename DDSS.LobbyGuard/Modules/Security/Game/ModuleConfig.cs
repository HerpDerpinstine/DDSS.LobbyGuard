using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Game
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> ServerOutageRandomMinimum;
        internal MelonPreferences_Entry<int> ServerOutageRandomMaximum;
        internal MelonPreferences_Entry<int> WorkstationVirusRandomMinimum;
        internal MelonPreferences_Entry<int> WorkstationVirusRandomMaximum;

        public ModuleConfig() : base()
            => Instance = this;
        public override eConfigType ConfigType
            => eConfigType.Security;
        public override string ID
            => "Game";
        public override string DisplayName
            => "Game";

        public override void CreatePreferences()
        {
            ServerOutageRandomMinimum = CreatePref("ServerOutageRandomMinimum",
                "Server Outage Random Minimum",
                "Minimum Value in Seconds for the Random Timer of Server Outages",
                120);

            ServerOutageRandomMaximum = CreatePref("ServerOutageRandomMaximum",
                "Server Outage Random Maximum",
                "Maximum Value in Seconds for the Random Timer of Server Outages",
                600);

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
