namespace DDSS_LobbyGuard.Modules.Security.Shelf
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Shelf";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}