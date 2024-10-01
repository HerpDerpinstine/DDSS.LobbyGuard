using DDSS_LobbyGuard.Patches;
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
            ApplyPatch<Patch_LobbyManager>();
            ApplyPatch<Patch_LobbyPlayer>();
            ApplyPatch<Patch_PhoneManager>();

            _logger.Msg("Initialized");
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
