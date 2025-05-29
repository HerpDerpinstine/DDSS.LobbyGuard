using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "MoreRoleSettings";
        public override eModuleType ModuleType => eModuleType.Extras;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}