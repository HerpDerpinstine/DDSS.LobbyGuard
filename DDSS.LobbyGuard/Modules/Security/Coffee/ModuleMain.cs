namespace DDSS_LobbyGuard.Modules.Security.Coffee
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Coffee";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}