﻿using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Server
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> ServerOutageRandomMinimum;
        internal MelonPreferences_Entry<int> ServerOutageRandomMaximum;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Server";
        public override string DisplayName
            => "Server";

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
        }
    }
}
