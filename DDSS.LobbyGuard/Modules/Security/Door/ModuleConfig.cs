using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Door
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> CloseDoorsOnLock;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Door";
        public override string DisplayName
            => "Door";

        public override void CreatePreferences()
        {
            CloseDoorsOnLock = CreatePref("CloseDoorsOnLock",
                "Close Doors On Lock",
                "Close Doors when Locking",
                true);
        }
    }
}
