namespace DDSS_LobbyGuard.GUI
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "GUI";

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;

            MainMenuPanelBuilder.MainMenuInit();
        }
    }
}
