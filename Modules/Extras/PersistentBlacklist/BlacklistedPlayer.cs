using System;

namespace DDSS_LobbyGuard.PersistentBlacklist
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