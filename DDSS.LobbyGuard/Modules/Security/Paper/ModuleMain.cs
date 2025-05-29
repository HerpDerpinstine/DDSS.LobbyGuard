using System;

namespace DDSS_LobbyGuard.Modules.Security.Paper
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Paper";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}