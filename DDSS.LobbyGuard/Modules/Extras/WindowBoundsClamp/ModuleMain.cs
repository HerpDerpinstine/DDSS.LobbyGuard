using System;

namespace DDSS_LobbyGuard.Modules.Extras.WindowBoundsClamp
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.WindowBoundsClamp";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}