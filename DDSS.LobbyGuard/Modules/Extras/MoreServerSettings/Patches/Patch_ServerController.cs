using DDSS_LobbyGuard.Modules.Extras.MoreServerSettings.Internal;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps.ServerRack;

namespace DDSS_LobbyGuard.Modules.Extras.MoreServerSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_ServerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ServerController), nameof(ServerController.Start))]
        private static bool Start_Prefix(ServerController __instance)
        {
            ServerController.connectionsEnabled = true;
            if (NetworkServer.activeHost)
                ServerSecurity.OnStart(__instance);

            // Prevent Original
            return false;
        }
    }
}
