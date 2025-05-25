using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Stereo
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> SpawnStereoCDs;

        public ModuleConfig() : base()
        {
            if (Instance == null)
                Instance = this;
        }
        public override void Init()
            => ConfigType = eConfigType.Security;
        public override string GetName()
            => "Stereo";
        public override string GetDisplayName()
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
