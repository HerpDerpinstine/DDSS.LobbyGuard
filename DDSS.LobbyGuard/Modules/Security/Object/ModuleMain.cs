namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleMain : ILobbyModule
    {
        public override string Name => "Object";
        public override eModuleType ModuleType => eModuleType.Security;

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            // Find All Static Objects

            // Get Position of Object

            // Find All Colliders

            // Get Collider Bounds

            // Cache Min/Max
        }
    }
}