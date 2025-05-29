namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Object";
        public override eModuleType ModuleType => eModuleType.Security;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            // Get Scene

            // Find All Colliders in Scene

            // Get Position + Offset of Collider

            // Get Collider Bounds

            // Cache Min/Max
        }
    }
}