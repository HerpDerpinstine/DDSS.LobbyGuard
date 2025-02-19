namespace DDSS_LobbyGuard.VersionCheck
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "VersionCheck";
        public override int Priority => 100;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;
            VersionCheck.Run();
        }
    }
}