namespace DDSS_LobbyGuard.Modules.Security.Pumpkin
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Pumpkin";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}