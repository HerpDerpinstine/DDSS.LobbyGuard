using DDSS_LobbyGuard.Modules.Security.Workstation.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Security.Workstation
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Workstation";

        public override bool OnLoad()
        {
            new Extras.MoreRoleSettings.ModuleConfig();
            new Extras.MoreWorkstationSettings.ModuleConfig();
            return true;
        }

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            ComputerSecurity._playerAddresses.Clear();
        }
    }
}