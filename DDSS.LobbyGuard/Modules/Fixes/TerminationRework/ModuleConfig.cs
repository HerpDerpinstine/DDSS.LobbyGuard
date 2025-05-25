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
        {
            if (Instance == null)
                Instance = this;
        }
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "PlayerLeavesReduceTerminations";
        public override string GetDisplayName()
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
