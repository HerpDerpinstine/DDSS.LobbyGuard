using System;

namespace DDSS_LobbyGuard.Modules.Security.HR
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "HR";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}