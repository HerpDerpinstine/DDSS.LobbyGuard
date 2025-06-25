using System;

namespace DDSS_LobbyGuard.Modules.Security.Server
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Server";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}