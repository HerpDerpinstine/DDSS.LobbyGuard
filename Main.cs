using DDSS_LobbyGuard.Patches;
using DDSS_LobbyGuard.Security;
using MelonLoader;
using System;

namespace DDSS_LobbyGuard
{
    internal class MelonMain : MelonMod
    {
        internal static MelonLogger.Instance _logger;

        public override void OnInitializeMelon()
        {
            _logger = LoggerInstance;

            ApplyPatch<Patch_GameManager>();
            ApplyPatch<Patch_LobbyItem>();
            ApplyPatch<Patch_LobbyManager>();
            ApplyPatch<Patch_LobbyPlayer>();
            ApplyPatch<Patch_PhoneManager>();
            ApplyPatch<Patch_ServerController>();
            ApplyPatch<Patch_SteamMatchmaking>();
            ApplyPatch<Patch_TrashBin>();

            _logger.Msg("Initialized!");
        }

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            PhoneSecurity.OnSceneLoad();
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
