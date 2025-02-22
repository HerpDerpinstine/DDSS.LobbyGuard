﻿using DDSS_LobbyGuard.Modules.Security.Player.Internal;

namespace DDSS_LobbyGuard.Modules.Security.Player
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Player"; 
        
        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            PlayerTriggerSecurity.OnSceneLoad();
        }
    }
}