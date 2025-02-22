using DDSS_LobbyGuard.Modules.Security.Door.Internal;

namespace DDSS_LobbyGuard.Modules.Security.Door
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Security.Door";

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene"
                && sceneName != "LobbyScene")
                return;
            DoorSecurity.OnSceneLoad();
        }
    }
}