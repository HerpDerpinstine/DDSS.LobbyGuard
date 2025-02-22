using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> WorkStationVirusTurnsOffFirewall;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreWorkstationSettings";
        public override string GetDisplayName()
            => "More Workstation Settings";

        public override void CreatePreferences()
        {
            WorkStationVirusTurnsOffFirewall = CreatePref("WorkStationVirusTurnsOffFirewall",
                "WorkStation Virus turns off Firewall",
                "WorkStation Viruses will Automatically Turn Off the Firewall",
                false);
        }
    }
}
