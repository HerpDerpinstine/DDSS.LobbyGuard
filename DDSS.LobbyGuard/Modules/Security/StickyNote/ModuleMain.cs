using System;

namespace DDSS_LobbyGuard.Modules.Security.StickyNote
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "StickyNote";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);
    }
}