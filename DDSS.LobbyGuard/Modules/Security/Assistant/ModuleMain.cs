using System;

namespace DDSS_LobbyGuard.Modules.Security.Assistant
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Assistant";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}