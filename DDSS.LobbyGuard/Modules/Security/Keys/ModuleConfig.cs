using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Keys
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> SpawnManagerKeys;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Security;
        public override string GetName()
            => "Keys";
        public override string GetDisplayName()
            => "Keys";

        public override void CreatePreferences()
        {
            SpawnManagerKeys = CreatePref("SpawnManagerKeys",
                "Spawn Manager Keys",
                "Allows Spawning of Keys in Manager's Office",
                true);
        }
    }
}
