using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Fixes.TerminationRework
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal enum eTermType
        {
            Always,
            Every_Other_Disconnect,
            Never
        }

        internal MelonPreferences_Entry<eTermType> PlayerLeavesReduceTerminations;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Fixes;
        public override string ID
            => "PlayerLeavesReduceTerminations";
        public override string DisplayName
            => "Player Leaves Reduce Terminations";

        public override void CreatePreferences()
        {
            PlayerLeavesReduceTerminations = CreatePref("PlayerLeavesReduceTerminations",
                "Player Leaves Reduce Terminations",
                "Allows Terminations to be Decreased from Players Disconnecting",
                eTermType.Always);
        }
    }
}
