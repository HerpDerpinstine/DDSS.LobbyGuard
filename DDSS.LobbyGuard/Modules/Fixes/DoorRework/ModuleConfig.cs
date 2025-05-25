using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Fixes.DoorRework
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> CloseDoorsOnLock;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Fixes;
        public override string GetName()
            => "DoorRework";
        public override string GetDisplayName()
            => "Door Rework";

        public override void CreatePreferences()
        {
            CloseDoorsOnLock = CreatePref("CloseDoorsOnLock",
                "Close Doors On Lock",
                "Close Doors when Locking",
                true);
        }
    }
}
