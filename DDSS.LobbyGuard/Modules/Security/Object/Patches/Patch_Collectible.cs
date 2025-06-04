using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using Il2CppProps.Smoking;
using Il2CppProps.WorkStation.InfectedUSB;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Collectible
    {
        private static Il2CppSystem.Type _floppyType1 = Il2CppType.Of<FloppyDiskController>();
        private static Il2CppSystem.Type _floppyType2 = Il2CppType.Of<InfectedUsb>();
        private static Il2CppSystem.Type _documentType = Il2CppType.Of<Document>();
        private static Il2CppSystem.Type _imageType = Il2CppType.Of<PrintedImage>();
        private static Il2CppSystem.Type _mugType = Il2CppType.Of<Mug>();
        private static Il2CppSystem.Type _paperReamType = Il2CppType.Of<PaperReam>();
        private static Il2CppSystem.Type _stickyType = Il2CppType.Of<StickyNoteController>();
        private static Il2CppSystem.Type _cigaretteType = Il2CppType.Of<Cigarette>();
        private static Il2CppSystem.Type _cigarettePackType = Il2CppType.Of<CigarettePack>();
        private static Il2CppSystem.Type _markerType = Il2CppType.Of<PermanentMarkerController>();
        private static Il2CppSystem.Type _inkType = Il2CppType.Of<InkCartridge>();
        private static Il2CppSystem.Type _toiletPaperType = Il2CppType.Of<ToiletPaper>();
        private static Il2CppSystem.Type _jellyType = Il2CppType.Of<Jelly>();
        private static Il2CppSystem.Type _fishFoodType = Il2CppType.Of<FishFoodController>();
        private static Il2CppSystem.Type _catFoodType = Il2CppType.Of<CatFoodController>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Collectible), nameof(Collectible.Start))]
        private static void Start_Postfix(Collectible __instance)
        {
            Il2CppSystem.Type collectibleType = __instance.GetIl2CppType();

            // Mugs
            if (collectibleType == _mugType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleMugs.Value;

            // Jelly
            if (collectibleType == _jellyType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleJelly.Value;

            // Cat Food
            if (collectibleType == _catFoodType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleCatFood.Value;

            // Fish Food
            if (collectibleType == _fishFoodType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleFishFood.Value;

            // Cigarettes
            if (collectibleType == _cigaretteType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleCigarettes.Value;

            // Cigarette Packs
            if (collectibleType == _cigarettePackType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleCigarettePacks.Value;

            // Floppy Disks
            if ((collectibleType == _floppyType1)
                || (collectibleType == _floppyType2))
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleFloppyDisks.Value;

            // Documents
            if (collectibleType == _documentType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleDocuments.Value;

            // Images
            if (collectibleType == _imageType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleImages.Value;

            // Paper Reams
            if (collectibleType == _paperReamType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdlePaperReams.Value;

            // Ink Cartridges
            if (collectibleType == _inkType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleInkCartridges.Value;

            // Permanent Markers
            if (collectibleType == _markerType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdlePermanentMarkers.Value;

            // Sticky Notes
            if (collectibleType == _stickyType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleStickyNotes.Value;

            // ToiletPaper
            if (collectibleType == _toiletPaperType)
                __instance.despawnAfterIdle = ModuleConfig.Instance.DespawnIdleToiletPaper.Value;
        }
    }
}