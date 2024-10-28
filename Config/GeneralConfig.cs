﻿using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GeneralConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> PromptForUpdateAvailable;
        internal MelonPreferences_Entry<bool> PromptForInitializationError;
        internal MelonPreferences_Entry<bool> ExtendedInviteCodes;

        internal override string GetName()
            => "General";

        internal override void CreatePreferences()
        {
            PromptForUpdateAvailable = CreatePref("PromptForUpdateAvailable",
                "Prompt For Update Available",
                "Checks to see if there is an Update available and Prompts if one is found",
                true);

            PromptForInitializationError = CreatePref("PromptForInitializationError",
                "Prompt For Initialization Error",
                "Shows a Prompt if an Initialization Error occurs",
                true);

            ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                true);
        }
    }
}