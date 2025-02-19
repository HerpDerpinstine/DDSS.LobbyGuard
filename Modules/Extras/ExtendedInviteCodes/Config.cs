using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.ExtendedInviteCodes
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> ExtendedInviteCodes;

        public ModuleConfig() : base() 
            => Instance = this;
        public override string GetName()
            => "Lobby";

        public override void CreatePreferences()
        {
            ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                true);
        }
    }
}
