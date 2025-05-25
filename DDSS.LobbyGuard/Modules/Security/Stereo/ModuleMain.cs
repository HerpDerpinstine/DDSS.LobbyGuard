using System;

namespace DDSS_LobbyGuard.Modules.Security.Stereo
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Stereo";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}