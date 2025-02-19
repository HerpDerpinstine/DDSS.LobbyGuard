using System;

namespace DDSS_LobbyGuard.Extras.PersistentBlacklist
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