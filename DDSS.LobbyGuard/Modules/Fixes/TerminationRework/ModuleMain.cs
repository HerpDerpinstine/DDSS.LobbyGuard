﻿using System;

namespace DDSS_LobbyGuard.Modules.Fixes.TerminationRework
{
    internal class ModuleMain : ILobbyModule
    {
        private static Random _rnd = new();
        internal static bool _canTerminate;

        public override string Name => "Fixes.TerminationRework";
        public override Type ConfigType => typeof(ModuleConfig);

        public override bool OnLoad()
        {
            new MoreJanitorSettingsConfig();
            return true;
        }

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;
            ResetTerminationChance();
        }

        internal static void ResetTerminationChance()
        {
            _canTerminate = _rnd.Next(0, 101) >= 50;
        }
    }
}