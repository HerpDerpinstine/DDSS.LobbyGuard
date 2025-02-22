using System;

namespace DDSS_LobbyGuard.Modules.Security.StickyNote
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.StickyNote";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}