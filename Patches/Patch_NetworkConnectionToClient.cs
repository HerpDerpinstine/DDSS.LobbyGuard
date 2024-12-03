using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_NetworkConnectionToClient
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(NetworkConnectionToClient), nameof(NetworkConnectionToClient.DestroyOwnedObjects))]
        private static bool DestroyOwnedObjects_Prefix(NetworkConnectionToClient __instance)
        {
            // Create Local Copy of Owned List
            HashSet<NetworkIdentity> ownedObjs = [.. __instance.owned];

            // Clear Owned List
            __instance.owned.Clear();

            // Iterate through Owned Objects
            foreach (NetworkIdentity networkIdentity in ownedObjs)
            {
                // Validate Identity
                if (networkIdentity == null)
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
                        // Prevent Destruction
                        ownedObjs.Remove(usable.netIdentity);

                        // Drop It
                        usable.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(networkIdentity, __instance);

                        // Manually Drop It on Server's Client
                        usable.UserCode_RpcOnStopUse__NetworkIdentity(networkIdentity);

                        // Change Ownership
                        usable.netIdentity.SetClientOwner(NetworkClient.localPlayer.connectionToClient);
                    }
                }
            }

            // Iterate through Owned Objects
            foreach (NetworkIdentity networkIdentity in ownedObjs)
            {
                // Validate Identity
                if (networkIdentity == null)
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
