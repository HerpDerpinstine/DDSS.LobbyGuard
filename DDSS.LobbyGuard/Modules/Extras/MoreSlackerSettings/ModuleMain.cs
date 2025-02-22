using DDSS_LobbyGuard.Modules.Extras.MoreSlackerSettings.Internal;
using System;

namespace DDSS_LobbyGuard.Modules.Extras.MoreSlackerSettings
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.MoreSlackerSettings";
        public override Type ConfigType => typeof(ModuleConfig);

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;

            TrashBinSecurity.OnSceneLoad();
        }
    }
}