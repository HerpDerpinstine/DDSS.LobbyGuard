using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Object
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> DespawnIdleMugs;
        internal MelonPreferences_Entry<bool> DespawnIdleJelly;
        internal MelonPreferences_Entry<bool> DespawnIdleCatFood;
        internal MelonPreferences_Entry<bool> DespawnIdleFishFood;
        internal MelonPreferences_Entry<bool> DespawnIdleCigarettes;
        internal MelonPreferences_Entry<bool> DespawnIdleCigarettePacks;
        internal MelonPreferences_Entry<bool> DespawnIdleFloppyDisks;
        internal MelonPreferences_Entry<bool> DespawnIdleDocuments;
        internal MelonPreferences_Entry<bool> DespawnIdleImages;
        internal MelonPreferences_Entry<bool> DespawnIdlePaperReams;
        internal MelonPreferences_Entry<bool> DespawnIdleInkCartridges;
        internal MelonPreferences_Entry<bool> DespawnIdlePermanentMarkers;
        internal MelonPreferences_Entry<bool> DespawnIdleStickyNotes;
        internal MelonPreferences_Entry<bool> DespawnIdleToiletPaper;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Object";
        public override string DisplayName
            => "Object";

        public override void CreatePreferences()
        {
            DespawnIdleMugs = CreatePref("DespawnIdleMugs",
                "Despawn Idle Mugs",
                "Mugs once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleJelly = CreatePref("DespawnIdleJelly",
                "Despawn Idle Jelly",
                "Jelly once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleCatFood = CreatePref("DespawnIdleCatFood",
                "Despawn Idle Cat Food",
                "Cat Food once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleFishFood = CreatePref("DespawnIdleFishFood",
                "Despawn Idle Fish Food",
                "Fish Food once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleCigarettes = CreatePref("DespawnIdleCigarettes",
                "Despawn Idle Cigarettes",
                "Cigarettes once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleCigarettePacks = CreatePref("DespawnIdleCigarettePacks",
                "Despawn Idle Cigarette Packs",
                "Cigarette Packs once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleFloppyDisks = CreatePref("DespawnIdleFloppyDisks",
                "Despawn Idle Floppy Disks",
                "Floppy Disks once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleDocuments = CreatePref("DespawnIdleDocuments",
                "Despawn Idle Documents",
                "Documents once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleImages = CreatePref("DespawnIdlePrintedImages",
                "Despawn Idle Images",
                "Images once Idle will Despawn after a certain period of time",
                true);

            DespawnIdlePaperReams = CreatePref("DespawnIdlePaperReams",
                "Despawn Idle Paper Reams",
                "Paper Reams once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleInkCartridges = CreatePref("DespawnIdleInkCartridges",
                "Despawn Idle Ink Cartridges",
                "Ink Cartridges once Idle will Despawn after a certain period of time",
                true);

            DespawnIdlePermanentMarkers = CreatePref("DespawnIdlePermanentMarkers",
                "Despawn Idle Permanent Markers",
                "Permanent Markers once Idle will Despawn after a certain period of time",
                true);
            
            DespawnIdleStickyNotes = CreatePref("DespawnIdleStickyNotes",
                "Despawn Idle Sticky Notes",
                "Sticky Notes once Idle will Despawn after a certain period of time",
                true);

            DespawnIdleToiletPaper = CreatePref("DespawnIdleToiletPaper",
                "Despawn Idle Toilet Paper",
                "Toilet Paper once Idle will Despawn after a certain period of time",
                true);
        }
    }
}
