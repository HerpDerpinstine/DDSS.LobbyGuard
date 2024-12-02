using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer.Lobby;
using Il2CppProps.Misc.PaperTray;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PaperTray
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperTray), nameof(PaperTray.InvokeUserCode_CmdSpawnDocument__String))]
        private static bool InvokeUserCode_CmdSpawnDocument__String_Prefix(
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Player
            LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
            if (player == null)
                return false;

            // Get Workstation
            WorkStationController station = player.NetworkworkStationController;
            if (station == null)
                return false;

            // Get PaperTray
            PaperTray tray = station.paperTray;
            if (tray == null) 
                return false;

            // Validate Free Slots
            int freeSlots = tray.freePositions.Count;
            if (!tray.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = tray.collectibles.Count;
            if (tray.allowStacking && (usedSlots >= InteractionSecurity.MAX_DOCUMENTS_TRAY))
                return false;

            // Get Value
            string document = __1.SafeReadString();
            if (string.IsNullOrEmpty(document)
                || string.IsNullOrWhiteSpace(document))
                return false;

            // Run Game Command
            tray.UserCode_CmdSpawnDocument__String(document);

            // Prevent Original
            return false;
        }
    }
}
