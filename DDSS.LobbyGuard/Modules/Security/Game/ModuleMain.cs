using System;

namespace DDSS_LobbyGuard.Modules.Security.Game
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Game";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}