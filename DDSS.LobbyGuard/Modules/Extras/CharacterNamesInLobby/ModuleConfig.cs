using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.CharacterNamesInLobby
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> CharacterNamesInPlayerList;
        internal MelonPreferences_Entry<bool> CharacterNamesInTextChat;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Extras;
        public override string ID
            => "CharacterNamesInLobby";
        public override string DisplayName
            => "Character Names in Lobby";

        public override void CreatePreferences()
        {
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
