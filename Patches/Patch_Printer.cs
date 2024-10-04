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
    internal class Patch_Printer
    {
        // InvokeUserCode_CmdPrintImage__Byte
        // UserCode_CmdPrintImage__Byte
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0))]
        private static bool Method_Protected_Static_Void_NetworkBehaviour_NetworkReader_NetworkConnectionToClient_PDM_0_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Validate Count
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
                return false;

            // Get Image Data
            byte[] imageData = __1.ReadBytesAndSize();
            if ((imageData == null)
                || (imageData.Length <= 0))
                return false;

            // Verify Image
            if (!PrinterSecurity.VerifyImage(printer, imageData))
                return false;

            // Run Game Command
            printer.Method_Protected_Void_Il2CppStructArray_1_Byte_PDM_0(imageData);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.InvokeUserCode_CmdPrintDocument__String__String))]
        private static bool InvokeUserCode_CmdPrintDocument__String__String_Prefix(
            NetworkBehaviour __0)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Validate Count
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
                return false;

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Printer), nameof(Printer.InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity))]
        private static bool InvokeUserCode_CmdCopyDocumentFromPlayer__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Printer
            Printer printer = __0.TryCast<Printer>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Count
            int freeSlots = printer.freePositions.Count;
            if (!printer.allowStacking && (freeSlots <= 0))
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
                printer.RpcSetImage(docCopy.netIdentity, doc.byteImg);
                docCopy.Networksigned = doc.Networksigned;
                docCopy.signed = doc.signed;
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
                printer.UserCode_CmdPlaceCollectible__NetworkIdentity__String(docCopy.netIdentity, docCopy.label);
                docCopy.SetText(doc.contentText.text);
                docCopy.SetName(doc.interactableName);
                docCopy.SetLabel(doc.label);
                docCopy.Networksigned = doc.Networksigned;
                docCopy.signed = doc.signed;
                docCopy.signedText.text = doc.signedText.text;
                docCopy.signedText.SetAllDirty();
            }

            // Prevent Original
            return false;
        }
    }
}