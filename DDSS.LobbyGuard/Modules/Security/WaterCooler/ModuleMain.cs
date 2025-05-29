namespace DDSS_LobbyGuard.Modules.Security.WaterCooler
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "WaterCooler";
        public override eModuleType ModuleType => eModuleType.Security;
    }
}