using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GeneralConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> ExtendedInviteCodes;

        internal override MelonPreferences_Category CreateCategory(string filePath)
        {
            var cat = MelonPreferences.CreateCategory("General", "General");
            cat.SetFilePath(filePath, true);
            return cat;
        }

        internal override void CreatePreferences()
        {
            ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                true);
        }
    }
}
