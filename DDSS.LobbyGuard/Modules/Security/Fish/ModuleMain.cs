using System;

namespace DDSS_LobbyGuard.Modules.Security.Fish
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Fish";
        public override eModuleType ModuleType => eModuleType.Security;
        public override bool OnLoad()
        {
            new Extras.MoreRoleSettings.ModuleConfig();
            return true;
        }
    }
}