using DDSS_LobbyGuard.Modules.Extras.MoreServerSettings.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreServerSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreServerSettings";
        public override Type ConfigType => typeof(ModuleConfig);
        
        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            ServerSecurity.OnSceneLoad();
        }
    }
}