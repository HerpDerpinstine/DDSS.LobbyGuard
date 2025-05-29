namespace DDSS_LobbyGuard.Modules.Security.CCTV
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "CCTV";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}