﻿using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GeneralConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> PromptForUpdateAvailable;
        internal MelonPreferences_Entry<bool> PromptForInitializationError;
        internal MelonPreferences_Entry<bool> PromptForBlacklistError;

        internal MelonPreferences_Entry<bool> UseServerTimeStampForEmails;
        internal MelonPreferences_Entry<bool> UseServerTimeStampForChatMessages;

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

            PromptForBlacklistError = CreatePref("PromptForBlacklistError",
                "Prompt For Blacklist Error",
                "Shows a Prompt if an Error occurs while Saving or Loading Blacklist.json",
                true);

            UseServerTimeStampForEmails = CreatePref("UseServerTimeStampForEmails",
                "Use Server TimeStamp for Emails",
                "Displays a Unified TimeStamp from the Server on Received Emails",
                true);

            UseServerTimeStampForChatMessages = CreatePref("UseServerTimeStampForChatMessages",
                "Use Server TimeStamp for Chat Messages",
                "Displays a Unified TimeStamp from the Server on Received Chat Messages",
                true);
        }
    }
}
