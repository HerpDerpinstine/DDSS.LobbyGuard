using System;

namespace DDSS_LobbyGuard.Modules.Security.Manager
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Manager";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);

        public override bool OnLoad()
        {
            new Extras.MoreRoleSettings.ModuleConfig();
            return true;
        }
    }
}