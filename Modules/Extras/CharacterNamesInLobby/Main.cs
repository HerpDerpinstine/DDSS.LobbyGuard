using Il2CppTMPro;
using Il2CppUI.Tabs.LobbyTab;
using System;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Extras.CharacterNamesInLobby
{
    internal class ModuleMain : ILobbyModule
    {
        private static Dictionary<PlayerLobbyUI, TextMeshProUGUI> _allCharacterNames = new();

        public override string Name => "Extras.CharacterNamesInLobby";
        public override Type ConfigType => typeof(ModuleConfig);

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if ((sceneName != "MainMenuScene")
                && (sceneName != "LobbyScene"))
                return;
            _allCharacterNames.Clear();
        }

        internal static void AddLobbyUICharacterName(PlayerLobbyUI ui, TextMeshProUGUI text)
            => _allCharacterNames[ui] = text;

        internal static TextMeshProUGUI GetLobbyUICharacterName(PlayerLobbyUI ui)
        {
            if (_allCharacterNames.TryGetValue(ui, out var text))
                return text;
            return null;
        }
    }
}