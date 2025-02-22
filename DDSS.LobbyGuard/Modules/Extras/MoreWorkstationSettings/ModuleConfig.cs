using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> WorkStationVirusResetsRandomVirusTimer;
        internal MelonPreferences_Entry<int> RandomWorkStationVirusDelayMin;
        internal MelonPreferences_Entry<int> RandomWorkStationVirusDelayMax;
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
            WorkStationVirusResetsRandomVirusTimer = CreatePref("WorkStationVirusResetsRandomVirusTimer",
                "WorkStation Virus resets Random Virus Timer",
                "WorkStation Viruses will Reset the Random Virus Timer",
                true);

            RandomWorkStationVirusDelayMin = CreatePref("RandomWorkStationVirusDelayMin",
                "Random WorkStation Virus Delay Min",
                "Min Delay in Seconds until a WorkStation rolls the Dice on getting a Random Virus",
                30);

            RandomWorkStationVirusDelayMax = CreatePref("RandomWorkStationVirusDelayMax",
                "Random WorkStation Virus Delay Max",
                "Max Delay in Seconds until a WorkStation rolls the Dice on getting a Random Virus",
                60);

            WorkStationVirusTurnsOffFirewall = CreatePref("WorkStationVirusTurnsOffFirewall",
                "WorkStation Virus turns off Firewall",
                "WorkStation Viruses will Automatically Turn Off the Firewall",
                false);
        }
    }
}
