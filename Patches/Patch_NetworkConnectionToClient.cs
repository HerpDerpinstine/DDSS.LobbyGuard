using HarmonyLib;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_NetworkConnectionToClient
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkConnectionToClient), nameof(NetworkConnectionToClient.DestroyOwnedObjects))]
        private static bool DestroyOwnedObjects_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
