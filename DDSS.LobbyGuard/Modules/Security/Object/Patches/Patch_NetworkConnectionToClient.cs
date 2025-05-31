using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Modules.Security.Player.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_NetworkConnectionToClient
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkConnectionToClient), nameof(NetworkConnectionToClient.DestroyOwnedObjects))]
        private static bool DestroyOwnedObjects_Prefix(NetworkConnectionToClient __instance)
        {
            if (__instance.owned == null)
                return false;

            // Create Local Copy of Owned List
            HashSet<NetworkIdentity> ownedObjs = [.. __instance.owned];

            // Clear Owned List
            __instance.owned.Clear();

            // Iterate through Owned Objects
            foreach (NetworkIdentity networkIdentity in ownedObjs)
            {
                // Validate Identity
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected)
                    continue;

                // Validate Player
                PlayerController player = networkIdentity.GetComponent<PlayerController>();
                if ((player != null)
                    && !player.WasCollected
                    && (player.currentUsables.Count > 0))
                {
                    // Get All Held Usables
                    foreach (Usable usable in player.currentUsables.ToArray())
                    {
                        // Validate Usable
                        if ((usable == null)
                            || usable.WasCollected)
                            continue;

                        // Prevent Destruction
                        ownedObjs.Remove(usable.netIdentity);

                        // Drop It
                        usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(networkIdentity, __instance);

                        // Manually Drop It on Server's Client
                        usable.UserCode_RpcOnStopUse__NetworkIdentity(networkIdentity);

                        // Change Ownership
                        usable.netIdentity.RemoveClientAuthority();
                    }
                }
            }

            // Iterate through Owned Objects
            foreach (NetworkIdentity networkIdentity in ownedObjs)
            {
                // Validate Identity
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected)
                    continue;

                // Validate Scene
                if (networkIdentity.sceneId != 0)
                    NetworkServer.UnSpawn(networkIdentity.gameObject);
                else
                    NetworkServer.Destroy(networkIdentity.gameObject);
            }

            // Prevent Original
            return false;
        }
    }
}