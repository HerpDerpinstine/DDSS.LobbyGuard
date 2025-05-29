using DDSS_LobbyGuard.Modules.Security.Workstation.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Security.Workstation
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Workstation";
        public override eModuleType ModuleType => eModuleType.Security;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            ComputerSecurity._playerAddresses.Clear();
        }
    }
}