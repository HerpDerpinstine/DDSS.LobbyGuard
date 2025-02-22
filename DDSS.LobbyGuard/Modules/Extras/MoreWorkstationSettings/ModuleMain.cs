using DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreWorkstationSettings";
        public override Type ConfigType => typeof(ModuleConfig);
        
        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            VirusHandler.OnSceneLoad();
        }
    }
}