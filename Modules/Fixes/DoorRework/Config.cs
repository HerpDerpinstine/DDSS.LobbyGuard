using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.DoorRework
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> CloseDoorsOnLock;

        public ModuleConfig() : base() 
            => Instance = this;
        public override string GetName()
            => "Gameplay";

        public override void CreatePreferences()
        {
            CloseDoorsOnLock = CreatePref("CloseDoorsOnLock",
                "Close Doors On Lock",
                "Close Doors when Locking",
                true);
        }
    }
}
