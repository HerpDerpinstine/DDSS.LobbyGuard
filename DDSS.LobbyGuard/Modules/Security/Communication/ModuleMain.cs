using System;

namespace DDSS_LobbyGuard.Modules.Security.Communication
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Communication";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}