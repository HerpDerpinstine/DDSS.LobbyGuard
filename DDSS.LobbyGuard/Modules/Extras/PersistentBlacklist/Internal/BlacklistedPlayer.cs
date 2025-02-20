using System;

namespace DDSS_LobbyGuard.Modules.Extras.PersistentBlacklist.Internal
{
    [Serializable]
    public class BlacklistedPlayer
    {
        public string Timestamp;
        public ulong SteamID;
        public string Name;
        public string Reason;
    }
}