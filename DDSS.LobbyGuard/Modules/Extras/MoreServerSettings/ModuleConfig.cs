using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreServerSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> SlackerServerOutageDelay;
        internal MelonPreferences_Entry<bool> ServerOutageResetsRandomOutageTimer;
        internal MelonPreferences_Entry<int> RandomServerOutageDelayMin;
        internal MelonPreferences_Entry<int> RandomServerOutageDelayMax;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreServerSettings";
        public override string GetDisplayName()
            => "More Server Settings";

        public override void CreatePreferences()
        {
            SlackerServerOutageDelay = CreatePref("SlackerServerOutageDelay",
                 "Slacker Server Outage Delay",
                 "Seconds until Server Outage from Slacker Task",
                 6);

            ServerOutageResetsRandomOutageTimer = CreatePref("ServerOutageResetsRandomOutageTimer",
                "Server Outage resets Random Outage Timer",
                "Server Outages will Reset the Random Outage Timer",
                false);

            RandomServerOutageDelayMin = CreatePref("RandomServerOutageDelayMin",
                "Random Server Outage Delay Min",
                "Min Delay in Seconds until the Server has a Random Outage",
                120);

            RandomServerOutageDelayMax = CreatePref("RandomServerOutageDelayMax",
                "Random Server Outage Delay Max",
                "Max Delay in Seconds until the Server has a Random Outage",
                600);
        }
    }
}
