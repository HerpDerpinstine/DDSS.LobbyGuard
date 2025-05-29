namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Object";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}