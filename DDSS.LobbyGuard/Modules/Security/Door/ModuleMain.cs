using DDSS_LobbyGuard.Modules.Security.Door.Internal;

namespace DDSS_LobbyGuard.Modules.Security.Door
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Door";
        public override eModuleType ModuleType => eModuleType.Security;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene"
                && sceneName != "LobbyScene")
                return;
            DoorSecurity.OnSceneLoad();
        }
    }
}