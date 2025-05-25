using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.WindowBoundsClamp
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> EnforceComputerWindowBoundary;

        public ModuleConfig() : base()
        {
            if (Instance == null)
                Instance = this;
        }
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "WindowBoundsClamp";
        public override string GetDisplayName()
            => "Window Bounds Clamp";

        public override void CreatePreferences()
        {
            EnforceComputerWindowBoundary = CreatePref("EnforceComputerWindowBoundary",
                "Enforce Computer Window Boundary",
                "Prevents Dragging Windows on WorkStation Computers outside of the Monitor's Boundaries",
                false);
        }
    }
}
