namespace DDSS_LobbyGuard.Modules.Security.Elevator
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Elevator";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}