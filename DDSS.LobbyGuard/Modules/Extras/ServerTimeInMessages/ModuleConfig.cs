using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.ServerTimeInMessages
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> UseServerTimeStampForEmails;
        internal MelonPreferences_Entry<bool> UseServerTimeStampForChatMessages;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "ServerTimeInMessages";
        public override string GetDisplayName()
            => "Server Time in Messages";

        public override void CreatePreferences()
        {
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
