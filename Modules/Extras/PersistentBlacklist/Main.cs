using System;

namespace DDSS_LobbyGuard.Extras.PersistentBlacklist
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.PersistentBlacklist";
        public override Type ConfigType => typeof(ModuleConfig);

        public override bool OnLoad()
        {
            BlacklistSecurity.Setup();
            return !BlacklistSecurity._error;
        }
    }
}