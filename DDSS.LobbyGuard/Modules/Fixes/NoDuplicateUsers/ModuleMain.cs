using System.Collections.Generic;

namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateUsers
{
    internal class ModuleMain : ILobbyModule
    {
        private static List<ulong> _validIds = new List<ulong>();

        public override string Name => "NoDuplicateUsers";
        public override eModuleType ModuleType => eModuleType.Fixes;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;
            _validIds.Clear();
        }

        internal static bool IsSteamIDInUse(ulong steamID)
            => _validIds.Contains(steamID);

        internal static void AddValidSteamID(ulong steamID)
        {
            if (IsSteamIDInUse(steamID))
                return;
            _validIds.Add(steamID);
        }

        internal static void RemoveValidSteamID(ulong steamID)
        {
            if (!IsSteamIDInUse(steamID))
                return;
            _validIds.Remove(steamID);
        }
    }
}