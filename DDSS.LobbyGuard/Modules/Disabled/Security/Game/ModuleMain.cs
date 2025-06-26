using System;

namespace DDSS_LobbyGuard.Modules.Security.Game
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Game";
        public override eModuleType ModuleType => eModuleType.Security;
        public override bool IsDisabled => true;
    }
}