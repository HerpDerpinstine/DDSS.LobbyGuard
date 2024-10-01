using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer.Lobby;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_LobbyManager
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.BlackListPlayer))]
        private static bool BlackListPlayer_Prefix(NetworkIdentity __0)
        {
            // Validate Player
            if ((__0 == null)
                || __0.WasCollected)
                return false;

            // Check for Local Player
            if (__0.isLocalPlayer
                || __0.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.KickPlayer))]
        private static bool KickPlayer_Prefix(NetworkIdentity __0)
        {
            // Validate Player
            if ((__0 == null)
                || __0.WasCollected)
                return false;

            // Check for Local Player
            if (__0.isLocalPlayer
                || __0.isServer)
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.InvokeUserCode_CmdForceManagerRole__NetworkIdentity))]
        private static bool InvokeUserCode_CmdForceManagerRole__NetworkIdentity_Prefix(NetworkConnectionToClient __2)
        {
            // Validate
            if ((__2 == null)
                || __2.WasCollected)
                return false;

            // Check for Lobby
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "LobbyScene")
            {
                // Check for Host
                if (!__2.identity.isServer)
                    return false;

                // Allow Host to Set in Lobby
                return true;
            }

            // Validate Manager Role when In-Game
            LobbyPlayer player = __2.identity.GetComponent<LobbyPlayer>();
            if ((player == null)
                || (player.playerRole != PlayerRole.Manager))
                return false;

            // Run Original
            return true;
        }
    }
}