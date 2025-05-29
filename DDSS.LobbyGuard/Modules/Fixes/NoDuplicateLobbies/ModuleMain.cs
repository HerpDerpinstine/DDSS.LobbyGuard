namespace DDSS_LobbyGuard.Modules.Fixes.NoDuplicateLobbies
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "NoDuplicateLobbies";
        public override eModuleType ModuleType => eModuleType.Fixes;
    }
}