using System.Collections.Generic;

namespace DDSS_LobbyGuard.Security
{
    internal static class LobbySecurity
    {
        private static List<ulong> _validIds = new List<ulong>();

        internal static void OnSceneLoad()
            => _validIds.Clear();

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
