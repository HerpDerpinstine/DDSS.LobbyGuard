using System;

namespace DDSS_LobbyGuard.Modules.Security.Communication
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Communication";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}