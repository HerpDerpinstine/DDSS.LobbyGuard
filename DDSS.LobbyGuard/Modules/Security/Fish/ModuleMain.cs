using System;

namespace DDSS_LobbyGuard.Modules.Security.Fish
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Fish";
        public override bool OnLoad()
        {
            new Extras.MoreRoleSettings.ModuleConfig();
            return true;
        }
    }
}