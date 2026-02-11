namespace DDSS_LobbyGuard.Modules.Security.Sink
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Sink";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}