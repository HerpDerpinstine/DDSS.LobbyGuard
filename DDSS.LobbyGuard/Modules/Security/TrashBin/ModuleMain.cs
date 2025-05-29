using System;

namespace DDSS_LobbyGuard.Modules.Security.TrashBin
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "TrashBin";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}