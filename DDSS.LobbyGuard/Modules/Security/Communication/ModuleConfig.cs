using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Communication
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<int> MaxCharactersOnChatMessages;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Security;
        public override string GetName()
            => "Communication";
        public override string GetDisplayName()
            => "Communication";

        public override void CreatePreferences()
        {
            MaxCharactersOnChatMessages = CreatePref("MaxCharactersOnChatMessages",
                "Max Characters on Chat Messages",
                "Max Characters allowed on Chat Messages",
                180);
        }
    }
}
