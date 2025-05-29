using Il2CppMirror;

namespace DDSS_LobbyGuard.Modules.Fixes.NoExceptionDisconnect
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "NoExceptionDisconnect";
        public override eModuleType ModuleType => eModuleType.Fixes;

        public override bool OnLoad()
        {
            NetworkServer.exceptionsDisconnect = false;
            NetworkClient.exceptionsDisconnect = false;
            return true;
        }
    }
}