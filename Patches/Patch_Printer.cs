using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Printer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.InvokeUserCode_CmdPrintDocument__String__String))]
        private static bool InvokeUserCode_CmdPrintDocument__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Sender Username
            string userName = sender.GetUserName();
            if (string.IsNullOrEmpty(userName)
                || string.IsNullOrWhiteSpace(userName))
                return false;

            // Validate Free Slots
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (printer.allowStacking && (usedSlots >= InteractionSecurity.MAX_DOCUMENTS_PRINTER))
                return false;

            // Get Document Content
            bool setLabel = true;
            string fileName = __1.ReadString();
            string documentContent = __1.ReadString().RemoveRichText();
            if (!string.IsNullOrEmpty(fileName)
                && !string.IsNullOrWhiteSpace(fileName))
            {
                TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
                if (textAsset != null)
                {
                    setLabel = false;
                    documentContent = textAsset.text;
                }
                else
                    fileName = "Document";
            }
            else
                fileName = "Document";
            if (string.IsNullOrEmpty(documentContent)
                || string.IsNullOrWhiteSpace(documentContent))
                return false;

            // Create New Document Copy
            Document docCopy = UnityEngine.Object.Instantiate(printer.documentPrefab,
                printer.printPos.position,
                printer.printPos.rotation);
            NetworkServer.Spawn(docCopy.gameObject);
            string newLabel = setLabel
                ? $"{userName.RemoveRichText()}'s Document"
                : fileName;
            if (newLabel == "SalesReport")
                newLabel = "Sales Report";
            printer.UserCode_CmdPlaceCollectible__NetworkIdentity__String(docCopy.netIdentity, newLabel);
            docCopy.SetLabel(newLabel);
            docCopy.SetText(documentContent);
            docCopy.SetName(setLabel ? newLabel : fileName);

            // Prevent Original
            return false;
        }

        // InvokeUserCode_CmdPrintImage__Byte
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0))]
        private static bool Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Sender Username
            string userName = sender.GetUserName();
            if (string.IsNullOrEmpty(userName)
                || string.IsNullOrWhiteSpace(userName))
                return false;

            // Validate Free Slots
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (printer.allowStacking && (usedSlots >= InteractionSecurity.MAX_DOCUMENTS_PRINTER))
                return false;

            // Get Image Data
            byte[] imageData = __1.ReadBytesAndSize();
            if ((imageData == null)
                || (imageData.Length <= 0))
                return false;

            // Verify Image
            if (!PrinterSecurity.VerifyImage(printer, imageData))
                return false;

            // Create New Document Copy
            string documentName = $"{userName.RemoveRichText()}'s Image";
            PrintedImage docCopy = GameObject.Instantiate<PrintedImage>(printer.printedImagePrefab,
                printer.printPos.position, printer.printPos.rotation);
            NetworkServer.Spawn(docCopy.gameObject);
            printer.UserCode_CmdPlaceCollectible__NetworkIdentity__String(docCopy.netIdentity, documentName);
            docCopy.SetLabel(documentName);
            printer.RpcSetImage(docCopy.netIdentity, imageData);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Free Slots
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (printer.allowStacking && (usedSlots >= InteractionSecurity.MAX_DOCUMENTS_PRINTER))
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, printer.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Validate Type
            Il2CppSystem.Type collectibleType = collectible.GetIl2CppType();
            if (collectibleType == Il2CppType.Of<PrintedImage>())
            {
                // Get Document
                PrintedImage doc = collectible.TryCast<PrintedImage>();
                if (doc == null)
                    return false;

                // Verify Image
                if (!PrinterSecurity.VerifyImage(printer, doc))
                    return false;

                // Create New Document Copy
                PrintedImage docCopy = GameObject.Instantiate<PrintedImage>(printer.printedImagePrefab,
                    printer.printPos.position, printer.printPos.rotation);
                NetworkServer.Spawn(docCopy.gameObject);
                printer.UserCode_CmdPlaceCollectible__NetworkIdentity__String(docCopy.netIdentity, doc.label);
                docCopy.SetName(doc.interactableName);
                docCopy.SetLabel(doc.label);
                printer.RpcSetImage(docCopy.netIdentity, doc.byteImg);
                docCopy.Networksigned = doc.Networksigned;
                docCopy.signed = doc.signed;
                if ((docCopy.signedText != null)
                    && !docCopy.signedText.WasCollected
                    && (doc.signedText != null)
                    && !doc.signedText.WasCollected)
                {
                    docCopy.signedText.text = doc.signedText.text;
                    docCopy.signedText.SetAllDirty();
                }
            }
            else if (collectibleType == Il2CppType.Of<Document>())
            {
                // Get Document
                Document doc = collectible.TryCast<Document>();
                if (doc == null)
                    return false;
                
                // Create New Document Copy
                Document docCopy = UnityEngine.Object.Instantiate(printer.documentPrefab,
                    printer.printPos.position,
                    printer.printPos.rotation);
                NetworkServer.Spawn(docCopy.gameObject);
                printer.UserCode_CmdPlaceCollectible__NetworkIdentity__String(docCopy.netIdentity, doc.label);
                docCopy.SetName(doc.interactableName);
                docCopy.SetText(doc.contentText.text);
                docCopy.SetLabel(doc.label);
                docCopy.Networksigned = doc.Networksigned;
                docCopy.signed = doc.signed;
                if ((docCopy.signedText != null)
                    && !docCopy.signedText.WasCollected
                    && (doc.signedText != null)
                    && !doc.signedText.WasCollected)
                {
                    docCopy.signedText.text = doc.signedText.text;
                    docCopy.signedText.SetAllDirty();
                }
            }

            // Prevent Original
            return false;
        }
    }
}