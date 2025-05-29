namespace DDSS_LobbyGuard.Modules.Security.Toilet
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Toilet";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}