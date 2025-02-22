using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreServerSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreServerSettings";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}