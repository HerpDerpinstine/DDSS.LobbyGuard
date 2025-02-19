using System;

namespace DDSS_LobbyGuard.PersistentBlacklist
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "PersistentBlacklist";
        public override Type ConfigType => typeof(ModuleConfig);

        public override bool OnLoad()
        {
            BlacklistSecurity.Setup();
            return !BlacklistSecurity._error;
        }
    }
}