using System;

namespace DDSS_LobbyGuard.Modules.Security.Phone
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Phone";
        public override eModuleType ModuleType => eModuleType.Security;

        public override bool OnLoad() => false;
    }
}