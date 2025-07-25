﻿using DDSS_LobbyGuard.Modules.General.GUI.Internal;

namespace DDSS_LobbyGuard.Modules.General.GUI
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "GUI";
        public override eModuleType ModuleType => eModuleType.General;
        public override int Priority => -100;

        public override void OnSceneInit(int buildIndex, string sceneName)
            => ModSettingsManager.SceneInit(sceneName);
    }
}
