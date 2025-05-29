using DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "PersistentBlacklist";
        public override eModuleType ModuleType => eModuleType.Extras;
        public override Type ConfigType => typeof(ModuleConfig);

        public override bool OnLoad()
        {
            BlacklistSecurity.Setup();
            return !BlacklistSecurity._error;
        }
    }
}