﻿using System;

namespace DDSS_LobbyGuard.Extras.ExtendedInviteCodes
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Extras.ExtendedInviteCodes";
        public override Type ConfigType => typeof(ModuleConfig);
    }
}