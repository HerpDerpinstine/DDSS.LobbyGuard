using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Fixes.NoExceptionDisconnect
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Fixes.NoExceptionDisconnect";

        public override bool OnLoad()
        {
            NetworkServer.exceptionsDisconnect = false;
            NetworkClient.exceptionsDisconnect = false;
            return true;
        }
    }
}