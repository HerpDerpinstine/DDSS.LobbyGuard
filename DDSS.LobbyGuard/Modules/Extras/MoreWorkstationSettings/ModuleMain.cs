using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreWorkstationSettings";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}