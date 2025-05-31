using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Manager
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> SpawnManagerKeys;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Manager";
        public override string DisplayName
            => "Manager";

        public override void CreatePreferences()
        {
            SpawnManagerKeys = CreatePref("SpawnManagerKeys",
                "Spawn Manager Keys",
                "Allows Spawning of Keys in Manager's Office",
                true);
        }
    }
}
