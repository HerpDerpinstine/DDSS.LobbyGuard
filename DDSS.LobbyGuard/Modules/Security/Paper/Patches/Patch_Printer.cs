﻿using DDSS_LobbyGuard.Modules.Security.Paper.Internal;
using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Paper.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Printer
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.Printer.Printer), nameof(Il2CppProps.Printer.Printer.InvokeUserCode_CmdFill__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFill__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkIdentity __0,
           NetworkConnectionToClient __2)
        {
            // Get Printer
            Il2CppProps.Printer.Printer printer = __0.TryCast<Il2CppProps.Printer.Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, printer))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate PaperReam
            PaperReam paperReam = collectible.TryCast<PaperReam>();
            if ((paperReam == null)
                || paperReam.WasCollected)
                return false;

            printer.UserCode_CmdFill__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.Printer.Printer), nameof(Il2CppProps.Printer.Printer.InvokeUserCode_CmdFillInk__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFillInk__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkIdentity __0,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Il2CppProps.Printer.Printer printer = __0.TryCast<Il2CppProps.Printer.Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, printer))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate InkCartridge
            InkCartridge ink = collectible.TryCast<InkCartridge>();
            if ((ink == null)
                || ink.WasCollected)
                return false;

            printer.UserCode_CmdFillInk__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.Printer.Printer), nameof(Il2CppProps.Printer.Printer.InvokeUserCode_CmdPrintDocument__String__String__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPrintDocument__String__String__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Il2CppProps.Printer.Printer printer = __0.TryCast<Il2CppProps.Printer.Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Get Sender Username
            string userName = sender.GetUserName();
            if (string.IsNullOrEmpty(userName)
                || string.IsNullOrWhiteSpace(userName))
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (usedSlots >= printer.maxPapers)
                return false;

            // Validate Sender is on Workstation
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected
                || (controller.NetworkcurrentChair == null)
                || controller.NetworkcurrentChair.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected)
                return false;

            // Get Document Content
            bool isPremadeDocument = false;
            string fileName = __1.SafeReadString();
            string documentContent = __1.SafeReadString().RemoveRichText();
            if (!string.IsNullOrEmpty(fileName)
                && !string.IsNullOrWhiteSpace(fileName))
            {
                TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
                if ((textAsset != null)
                    && !textAsset.WasCollected)
                {
                    isPremadeDocument = true;
                    documentContent = textAsset.text;
                }
                else
                    fileName = "Document";
            }
            else
                fileName = "Document";

            if (!isPremadeDocument)
            {
                if (string.IsNullOrEmpty(documentContent)
                    || string.IsNullOrWhiteSpace(documentContent))
                    return false;

                if (documentContent.Length > InteractionSecurity.MAX_DOCUMENT_CHARS)
                    documentContent = documentContent.Substring(0, InteractionSecurity.MAX_DOCUMENT_CHARS);

                if (string.IsNullOrEmpty(documentContent)
                    || string.IsNullOrWhiteSpace(documentContent))
                    return false;
            }

            // Create New Document Copy
            GameObject docObj = GameObject.Instantiate(printer.documentPrefab.gameObject,
                printer.printPos.position,
                printer.printPos.rotation);
            if ((docObj == null)
                || docObj.WasCollected)
                return false;

            // Get New Document
            Document docCopy = docObj.GetComponent<Document>();
            if ((docCopy == null)
                || docCopy.WasCollected)
                return false;

            // Spawn the Object on the Server
            NetworkServer.Spawn(docObj);

            // Get New Label
            string newLabel = fileName;
            if (newLabel == "SalesReport")
                newLabel = "Sales Report";
            string playerLabel = isPremadeDocument
                ? newLabel
                : $"{userName.RemoveRichText()}'s Document";

            // Apply Label and Text
            docCopy.SetText(documentContent);
            docCopy.SetName(fileName);
            switch (ModuleConfig.Instance.UsernamesOnPrintedDocuments.Value)
            {
                case Config.eConfigHostType.ALL:
                    docCopy.SetLabel(playerLabel);
                    printer.ServerPlaceCollectible(docCopy.netIdentity, playerLabel);
                    break;

                case Config.eConfigHostType.HOST_ONLY:
                    docCopy.SetLabel(newLabel);
                    printer.ServerPlaceCollectible(docCopy.netIdentity, newLabel);
                    docCopy.label = playerLabel;
                    docCopy.UpdateCollectible();
                    break;

                case Config.eConfigHostType.DISABLED:
                default:
                    docCopy.SetLabel(newLabel);
                    printer.ServerPlaceCollectible(docCopy.netIdentity, newLabel);
                    break;
            }

            // Prevent Original
            return false;
        }

        // InvokeUserCode_CmdPrintImage__Byte
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.Printer.Printer), nameof(Il2CppProps.Printer.Printer.Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0))]
        private static bool Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Il2CppProps.Printer.Printer printer = __0.TryCast<Il2CppProps.Printer.Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Get Sender Username
            string userName = sender.GetUserName();
            if (string.IsNullOrEmpty(userName)
                || string.IsNullOrWhiteSpace(userName))
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (usedSlots >= printer.maxPapers)
                return false;

            // Validate Sender is on Workstation
            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected
                || (controller.NetworkcurrentChair == null)
                || controller.NetworkcurrentChair.WasCollected)
                return false;

            // Validate Chair
            WorkStationController workStation = controller.NetworkcurrentChair.GetComponent<WorkStationController>();
            if ((workStation == null)
                || workStation.WasCollected)
                return false;

            // Get Image Data
            byte[] imageData = __1.SafeReadBytesAndSize();
            if ((imageData == null)
                || (imageData.Length <= 0))
                return false;

            // Verify Image
            if (!PrinterSecurity.VerifyImage(printer, imageData))
                return false;

            // Create New Document Copy
            GameObject imgObj = GameObject.Instantiate(printer.printedImagePrefab.gameObject,
                printer.printPos.position, printer.printPos.rotation);
            if ((imgObj == null)
                || imgObj.WasCollected)
                return false;

            // Get New Image
            PrintedImage imgCopy = imgObj.GetComponent<PrintedImage>();
            if ((imgCopy == null)
                || imgCopy.WasCollected)
                return false;

            // Spawn the Object on the Server
            NetworkServer.Spawn(imgObj);

            // Get New Label
            string fileName = "Printed Image";
            string newLabel = fileName;
            string playerLabel = $"{userName.RemoveRichText()}'s Image";

            // Apply Label and Text
            printer.RpcSetImage(imgCopy.netIdentity, imageData);
            imgCopy.SetName(fileName);
            switch (ModuleConfig.Instance.UsernamesOnPrintedImages.Value)
            {
                case Config.eConfigHostType.ALL:
                    imgCopy.SetLabel(playerLabel);
                    printer.ServerPlaceCollectible(imgCopy.netIdentity, playerLabel);
                    break;

                case Config.eConfigHostType.HOST_ONLY:
                    imgCopy.SetLabel(newLabel);
                    printer.ServerPlaceCollectible(imgCopy.netIdentity, newLabel);
                    imgCopy.label = playerLabel;
                    imgCopy.UpdateCollectible();
                    break;

                case Config.eConfigHostType.DISABLED:
                default:
                    imgCopy.SetLabel(newLabel);
                    printer.ServerPlaceCollectible(imgCopy.netIdentity, newLabel);
                    break;
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2CppProps.Printer.Printer), nameof(Il2CppProps.Printer.Printer.InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Il2CppProps.Printer.Printer printer = __0.TryCast<Il2CppProps.Printer.Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Used Slots
            int usedSlots = printer.collectibles.Count;
            if (usedSlots >= printer.maxPapers)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, printer))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Type
            Il2CppSystem.Type collectibleType = collectible.GetIl2CppType();
            if (collectibleType == Il2CppType.Of<PrintedImage>())
            {
                // Get Document
                PrintedImage img = collectible.TryCast<PrintedImage>();
                if (img == null)
                    return false;

                // Verify Image
                if (!PrinterSecurity.VerifyImage(printer, img))
                    return false;

                // Create New Image Copy
                GameObject imgObj = GameObject.Instantiate(printer.printedImagePrefab.gameObject,
                    printer.printPos.position, printer.printPos.rotation);
                if ((imgObj == null)
                    || imgObj.WasCollected)
                    return false;

                // Get New Image
                PrintedImage imgCopy = imgObj.GetComponent<PrintedImage>();
                if ((imgCopy == null)
                    || imgCopy.WasCollected)
                    return false;

                // Spawn the Object on the Server
                NetworkServer.Spawn(imgObj);

                // Set Label and Image
                imgCopy.SetLabel(img.label);
                imgCopy.SetName(img.interactableName);
                printer.RpcSetImage(imgCopy.netIdentity, img.byteImg);

                if (ModuleConfig.Instance.PrinterCopiesSignatures.Value)
                {
                    // Set Signed State
                    imgCopy.Networksigned = img.Networksigned;
                    imgCopy.signed = img.signed;

                    // Set Signed Text
                    if ((imgCopy.signedText != null)
                        && !imgCopy.signedText.WasCollected
                        && (img.signedText != null)
                        && !img.signedText.WasCollected)
                    {
                        imgCopy.signedText.text = img.signedText.text;
                        imgCopy.signedText.SetAllDirty();
                    }
                }

                // Place Image in Printer
                printer.ServerPlaceCollectible(imgCopy.netIdentity, img.label);
            }
            else if (collectibleType == Il2CppType.Of<Document>())
            {
                // Get Document
                Document doc = collectible.TryCast<Document>();
                if (doc == null)
                    return false;

                // Create New Document Copy
                GameObject docObj = GameObject.Instantiate(printer.documentPrefab.gameObject,
                    printer.printPos.position,
                    printer.printPos.rotation);
                if ((docObj == null)
                    || docObj.WasCollected)
                    return false;

                // Get New Document
                Document docCopy = docObj.GetComponent<Document>();
                if ((docCopy == null)
                    || docCopy.WasCollected)
                    return false;

                // Spawn the Object on the Server
                NetworkServer.Spawn(docCopy.gameObject);

                // Set Label and Text
                docCopy.SetLabel(doc.label);
                docCopy.SetName(doc.interactableName);
                docCopy.SetText(doc.contentText.text);

                if (ModuleConfig.Instance.PrinterCopiesSignatures.Value)
                {
                    // Set Signed State
                    docCopy.Networksigned = doc.Networksigned;
                    docCopy.signed = doc.signed;

                    // Set Signed Text
                    if ((docCopy.signedText != null)
                        && !docCopy.signedText.WasCollected
                        && (doc.signedText != null)
                        && !doc.signedText.WasCollected)
                    {
                        docCopy.signedText.text = doc.signedText.text;
                        docCopy.signedText.SetAllDirty();
                    }
                }

                // Place Image in Printer
                printer.ServerPlaceCollectible(docCopy.netIdentity, doc.label);
            }

            // Prevent Original
            return false;
        }

    }
}
