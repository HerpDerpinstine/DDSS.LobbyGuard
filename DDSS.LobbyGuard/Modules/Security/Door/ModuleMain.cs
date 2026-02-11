using DDSS_LobbyGuard.Modules.Security.Door.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Security.Door
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Door";
        public override eModuleType ModuleType => eModuleType.Security;
        public override Type ConfigType => typeof(ModuleConfig);

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene"
                && sceneName != "LobbyScene")
                return;
            DoorSecurity.OnSceneLoad();
        }
    }
}