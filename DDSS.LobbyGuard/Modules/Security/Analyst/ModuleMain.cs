using System;

namespace DDSS_LobbyGuard.Modules.Security.Analyst
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Analyst";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}