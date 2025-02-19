namespace DDSS_LobbyGuard.NoDuplicateUsers
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "NoDuplicateUsers";

#if DEBUG
        public override bool OnLoad() => false;
#endif
    }
}