using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class LobbyConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> ExtendedInviteCodes;
        internal MelonPreferences_Entry<bool> CharacterNamesInPlayerList;
        internal MelonPreferences_Entry<bool> CharacterNamesInTextChat;

        internal override string GetName()
            => "Lobby";

        internal override void CreatePreferences()
        {
            ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                true);

            CharacterNamesInPlayerList = CreatePref("CharacterNamesInPlayerList",
                "Character Names in Player List",
                "Shows Character Names in the Lobby Player List",
                false);

            CharacterNamesInTextChat = CreatePref("CharacterNamesInTextChat",
                "Character Names in Text Chat",
                "Shows Character Names in the Lobby Text Chat",
                false);
        }
    }
}
