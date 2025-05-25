using System;

namespace DDSS_LobbyGuard.Modules.Security.Paper
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Paper";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}