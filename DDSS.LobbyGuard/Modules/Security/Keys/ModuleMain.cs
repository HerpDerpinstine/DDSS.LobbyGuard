using System;

namespace DDSS_LobbyGuard.Modules.Security.Keys
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Keys";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}