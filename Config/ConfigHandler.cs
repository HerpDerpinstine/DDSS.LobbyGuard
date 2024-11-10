namespace DDSS_LobbyGuard.Config
{
    internal static class ConfigHandler
    {
        internal static GeneralConfig General;
        internal static ModerationConfig Moderation;
        internal static LobbyConfig Lobby;
        internal static GameplayConfig Gameplay;

        internal static void Setup()
        {
            if (General == null)
                General = new();
            if (Moderation == null)
                Moderation = new();
            if (Lobby == null)
                Lobby = new();
            if (Gameplay == null)
                Gameplay = new();
        }
    }
}
