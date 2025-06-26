using System;

namespace DDSS_LobbyGuard.Modules.Fixes.MatchInitRework
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "MatchInitRework";
        public override eModuleType ModuleType => eModuleType.Fixes;
        public override bool IsDisabled => true;
    }
}