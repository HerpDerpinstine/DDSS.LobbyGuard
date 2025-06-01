using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> DespawnIdleCollectibles;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Object";
        public override string DisplayName
            => "Object";

        public override void CreatePreferences()
        {
            DespawnIdleCollectibles = CreatePref("DespawnIdleCollectibles",
                "Despawn Idle Collectibles",
                "Collectibles once Idle will Despawn after a certain period of time",
                true);
        }
    }
}
