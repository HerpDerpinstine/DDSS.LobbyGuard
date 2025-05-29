using System;

namespace DDSS_LobbyGuard.Modules.Extras.ServerTimeInMessages
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "ServerTimeInMessages";
        public override eModuleType ModuleType => eModuleType.Extras;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}