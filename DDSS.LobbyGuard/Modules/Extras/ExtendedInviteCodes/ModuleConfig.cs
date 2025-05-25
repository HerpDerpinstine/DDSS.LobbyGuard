using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.ExtendedInviteCodes
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> ExtendedInviteCodes;

        public ModuleConfig() : base()
        {
            if (Instance == null)
                Instance = this;
        }
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "ExtendedInviteCodes";
        public override string GetDisplayName()
            => "Extended Invite Codes";

        public override void CreatePreferences()
        {
            ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                true);
        }
    }
}
