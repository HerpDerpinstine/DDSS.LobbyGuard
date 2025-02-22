using System;

namespace DDSS_LobbyGuard.Modules.Security.Printer
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Printer";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}