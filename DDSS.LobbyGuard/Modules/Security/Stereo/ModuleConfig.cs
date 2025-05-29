using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Stereo
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> SpawnStereoCDs;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Stereo";
        public override string DisplayName
            => "Stereo";

        public override void CreatePreferences()
        {
            SpawnStereoCDs = CreatePref("SpawnStereoCDs",
                "Spawn Stereo CDs",
                "Allows Spawning of CDs next to the Stereo",
                true);
        }
    }
}
