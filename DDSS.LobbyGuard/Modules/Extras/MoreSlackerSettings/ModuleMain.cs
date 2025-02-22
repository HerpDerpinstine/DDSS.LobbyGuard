using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreSlackerSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreSlackerSettings";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}