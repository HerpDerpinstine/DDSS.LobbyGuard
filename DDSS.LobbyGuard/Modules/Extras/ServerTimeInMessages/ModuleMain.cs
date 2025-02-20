using System;

namespace DDSS_LobbyGuard.Modules.Extras.ServerTimeInMessages
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.ServerTimeInMessages";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}