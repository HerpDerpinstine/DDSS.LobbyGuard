using System;

namespace DDSS_LobbyGuard.Modules.Security.Stereo
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Stereo";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}