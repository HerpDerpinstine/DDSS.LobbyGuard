using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreRoleSettings";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}