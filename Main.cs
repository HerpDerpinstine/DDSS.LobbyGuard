using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Patches;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using MelonLoader;
using MelonLoader.Utils;
using System;
using System.IO;

namespace DDSS_LobbyGuard
{
    internal class MelonMain : MelonMod
    {
        internal static string _userDataPath;
        internal static MelonLogger.Instance _logger;

        public override void OnInitializeMelon()
        {
            // Cache Logger 
            _logger = LoggerInstance;

            // Setup UserData Folder
            _userDataPath = Path.Combine(MelonEnvironment.UserDataDirectory, Properties.BuildInfo.Name);
            if (!Directory.Exists(_userDataPath))
                Directory.CreateDirectory(_userDataPath);

            // Setup Config
            ConfigHandler.Setup();
            BlacklistSecurity.Setup();

            // Register Custom Components
            ManagedEnumerator.Register();

            // Apply Patches
            ApplyPatch<Patch_BeerController>();
            ApplyPatch<Patch_Binder>();
            ApplyPatch<Patch_CatController>();
            ApplyPatch<Patch_Cigarette>();
            ApplyPatch<Patch_CigarettePack>();
            ApplyPatch<Patch_CoffeeMachine>();
            ApplyPatch<Patch_CollectibleHolder>();
            ApplyPatch<Patch_Consumable>();
            ApplyPatch<Patch_Document>();
            ApplyPatch<Patch_DoorController>();
            ApplyPatch<Patch_DrawerController>();
            ApplyPatch<Patch_Fax>();
            ApplyPatch<Patch_FizzySteamworks>();
            ApplyPatch<Patch_KitchenCabinetController>();
            ApplyPatch<Patch_LobbyItem>();
            ApplyPatch<Patch_LobbyManager>();
            ApplyPatch<Patch_LobbyPlayer>();
            ApplyPatch<Patch_Mug>();
            ApplyPatch<Patch_NetworkManager>();
            ApplyPatch<Patch_OfficeShelf>();
            ApplyPatch<Patch_PaperShredder>();
            ApplyPatch<Patch_PaperTray>();
            ApplyPatch<Patch_PhoneManager>();
            ApplyPatch<Patch_PlayerController>();
            ApplyPatch<Patch_PlayerEffectController>();
            ApplyPatch<Patch_Printer>();
            ApplyPatch<Patch_ServerController>();
            ApplyPatch<Patch_SteamMatchmaking>();
            ApplyPatch<Patch_StickyNoteController>();
            ApplyPatch<Patch_StorageBox>();
            ApplyPatch<Patch_Toilet>();
            ApplyPatch<Patch_TrashBin>();
            ApplyPatch<Patch_Usable>();
            ApplyPatch<Patch_VendingMachine>();
            ApplyPatch<Patch_VirusController>();
            ApplyPatch<Patch_VoteBoxController>();
            ApplyPatch<Patch_WorkStationController>();

            // Log Success
            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            DoorSecurity.OnSceneLoad();
            PhoneSecurity.OnSceneLoad();
            InteractionSecurity.UpdateSettings();
            ServerSecurity.OnSceneLoad();
            TrashBinSecurity.OnSceneLoad();
        }

        private void ApplyPatch<T>()
        {
            Type type = typeof(T);
            try
            {
                HarmonyInstance.PatchAll(type);
            }
            catch (Exception e)
            {
                LoggerInstance.Error($"Exception while attempting to apply {type.Name}: {e}");
            }
        }
    }   
}
