using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2CppMirror;
using Il2CppProps;
using Il2CppProps.Printer;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_OfficeShelf
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(OfficeShelf), nameof(OfficeShelf.InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32))]
        private static bool InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32_Prefix(NetworkReader __1)
        {
            // Get Document
            string document = __1.ReadString();
            if (string.IsNullOrEmpty(document)
                || string.IsNullOrWhiteSpace(document))
                return false;

            // Get FileName
            string fileName = __1.ReadString();
            if (string.IsNullOrEmpty(fileName)
                || string.IsNullOrWhiteSpace(fileName))
                return false;

            // Get Document Category
            DocumentCategory documentCategory = (DocumentCategory)__1.ReadInt();

            // Get Binder
            int binder = __1.ReadInt();
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
            string documentContent = Resources.Load<TextAsset>("files/" + fileName).text;
            if (string.IsNullOrEmpty(documentContent)
                || string.IsNullOrWhiteSpace(documentContent))
                return false;

            // Run Game Command
            binderObj.UserCode_CmdAddDocument__String__String(document, documentContent);

            // Prevent Original
            return false;
        }
    }
}
