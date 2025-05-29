using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> AllowVirusesWhileServerIsDown;
        internal MelonPreferences_Entry<bool> WorkStationVirusTurnsOffFirewall;
        internal MelonPreferences_Entry<bool> EnforceComputerWindowBoundary;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Extras;
        public override string ID
            => "MoreWorkstationSettings";
        public override string DisplayName
            => "More Workstation Settings";

        public override void CreatePreferences()
        {
            AllowVirusesWhileServerIsDown = CreatePref("AllowVirusesWhileServerIsDown",
                "Allow Viruses while Server is Down",
                "Allows WorkStations to get Viruses while the Server is Down",
                true);

            WorkStationVirusTurnsOffFirewall = CreatePref("WorkStationVirusTurnsOffFirewall",
                "WorkStation Virus turns off Firewall",
                "WorkStation Viruses will Automatically Turn Off the Firewall",
                false);

            EnforceComputerWindowBoundary = CreatePref("EnforceComputerWindowBoundary",
                "Enforce Computer Window Boundary",
                "Prevents Dragging Windows on WorkStation Computers outside of the Monitor's Boundaries",
                false);
        }
    }
}
