namespace DDSS_LobbyGuard.Fixes.NoDuplicateUsers
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Fixes.NoDuplicateUsers";

#if DEBUG
        public override bool OnLoad() => false;
#endif
    }
}