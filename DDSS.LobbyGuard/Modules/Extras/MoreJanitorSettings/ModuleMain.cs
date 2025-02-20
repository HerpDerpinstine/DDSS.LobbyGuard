using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreJanitorSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreJanitorSettings";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}