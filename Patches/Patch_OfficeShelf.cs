using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps;
using Il2CppProps.Printer;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_OfficeShelf
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OfficeShelf), nameof(OfficeShelf.InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32))]
        private static bool InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32_Prefix(NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get Document
            string document = __1.SafeReadString();
            if (string.IsNullOrEmpty(document)
                || string.IsNullOrWhiteSpace(document))
                return false;

            // Get FileName
            string fileName = __1.SafeReadString();
            if (string.IsNullOrEmpty(fileName)
                || string.IsNullOrWhiteSpace(fileName))
                return false;

            // Get Document Category
            DocumentCategory documentCategory = (DocumentCategory)__1.SafeReadInt();

            // Get Binder
            int binder = __1.SafeReadInt();
            if (binder < 0)
                return false;

            // Validate Binder Manager
            if ((BinderManager.instance == null)
                || (BinderManager.instance.binders == null)
                || !BinderManager.instance.binders.ContainsKey(documentCategory))
                return false;

            // Get Binder Category
            var binderCat = BinderManager.instance.binders[documentCategory];
            if ((binderCat == null)
                || (binderCat.Count <= 0))
                return false;

            // Validate Binder
            int binderCount = binderCat.Count - 1;
            if (binder > binderCount)
                return false;

            // Get Binder
            Binder binderObj = binderCat[binder];
            if (binderObj == null) 
                return false;

            // Validate Count
            if (binderObj.documents.Count >= InteractionSecurity.MAX_DOCUMENTS_BINDER)
                return false;

            // Get Document Content
            TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
            if (textAsset == null)
                return false;

            string documentContent = textAsset.text;
            if (string.IsNullOrEmpty(documentContent)
                || string.IsNullOrWhiteSpace(documentContent))
                return false;

            // Run Game Command
            binderObj.UserCode_CmdAddDocumentServer__String__String(document, documentContent);

            // Prevent Original
            return false;
        }
    }
}
