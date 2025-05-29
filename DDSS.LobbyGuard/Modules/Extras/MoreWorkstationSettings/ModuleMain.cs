using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "MoreWorkstationSettings";
        public override eModuleType ModuleType => eModuleType.Extras;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}