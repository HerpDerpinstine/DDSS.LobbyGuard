using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.Misc.PaperTray;
using Il2CppProps.Printer;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Paper.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_PaperTray
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PaperTray), nameof(PaperTray.InvokeUserCode_CmdSpawnDocument__String__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSpawnDocument__String__NetworkConnectionToClient_Prefix(
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if (controller == null
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if (lobbyPlayer == null
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Get Workstation
            WorkStationController station = lobbyPlayer.NetworkworkStationController;
            if (station == null)
                return false;

            // Get PaperTray
            PaperTray tray = station.paperTray;
            if (tray == null)
                return false;

            // Validate Free Slots
            int freeSlots = tray.freePositions.Count;
            if (!tray.allowStacking && freeSlots <= 0)
                return false;

            // Validate Used Slots
            int usedSlots = tray.collectibles.Count;
            if (tray.allowStacking && usedSlots >= InteractionSecurity.MAX_DOCUMENTS_TRAY)
                return false;

            // Get Value
            string document = __1.SafeReadString();
            if (string.IsNullOrEmpty(document)
                || string.IsNullOrWhiteSpace(document))
                return false;

            TextAsset textAsset = Resources.Load<TextAsset>("files/" + document);
            if (textAsset == null
                || textAsset.WasCollected)
                return false;

            string documentContent = textAsset.text;
            if (string.IsNullOrEmpty(documentContent)
                || string.IsNullOrWhiteSpace(documentContent))
                return false;

            // Create New Document Copy
            GameObject docObj = UnityEngine.Object.Instantiate(tray.documentPrefab.gameObject,
                tray.transform.position,
                tray.transform.rotation);
            if (docObj == null
                || docObj.WasCollected)
                return false;

            // Get New Document
            Document docCopy = docObj.GetComponent<Document>();
            if (docCopy == null
                || docCopy.WasCollected)
                return false;

            // Spawn the Object on the Server
            NetworkServer.Spawn(docObj);

            // Apply Label and Text
            docCopy.SetLabel(document);
            docCopy.SetText(documentContent);
            docCopy.SetName(document);

            // Place Document in Printer
            tray.ServerPlaceCollectible(docCopy.netIdentity, document);

            // Prevent Original
            return false;
        }
    }
}