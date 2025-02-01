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
        [HarmonyPatch(typeof(OfficeShelf), nameof(OfficeShelf.Start))]
        private static bool Start_Prefix(OfficeShelf __instance)
        {
            // Add Shelf to BinderManager
            if (!BinderManager.instance.shelves.ContainsKey(__instance.shelfCategory))
                BinderManager.instance.shelves.Add(__instance.shelfCategory, __instance);

            // Spawn Binders
            if (NetworkServer.activeHost)
                CollectibleSecurity.SpawnBinderStart(__instance);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OfficeShelf), nameof(OfficeShelf.InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSpawnDocument__String__String__DocumentCategory__Int32__NetworkConnectionToClient_Prefix(NetworkBehaviour __0, NetworkReader __1, NetworkConnectionToClient __2)
        {
            // Get OfficeShelf
            OfficeShelf shelf = __0.TryCast<OfficeShelf>();

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
            int docCatId = __1.SafeReadInt();
            DocumentCategory documentCategory = (DocumentCategory)docCatId;

            // Validate Binder Manager
            if ((BinderManager.instance == null)
                || (BinderManager.instance.binders == null)
                || !BinderManager.instance.binders.ContainsKey(documentCategory))
                return false;

            var binderCat = BinderManager.instance.binders[documentCategory];
            int binderCount = binderCat.Count - 1;

            int binder = __1.SafeReadByte();
            if (binder < 0)
                binder = 0;
            if (binder > binderCount)
                binder = binderCount;

            // Get Binder
            Binder binderObj = binderCat[binder];
            if (binderObj == null) 
                return false;

            // Validate Count
            if (binderObj.documents.Count >= InteractionSecurity.MAX_DOCUMENTS_BINDER)
                return false;

            // Get Document Content
            TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
            if ((textAsset == null)
                || textAsset.WasCollected)
                return false;

            string documentContent = textAsset.text;
            if (string.IsNullOrEmpty(documentContent)
                || string.IsNullOrWhiteSpace(documentContent))
                return false;

            // Run Game Command
            binderObj.ServerAddDocument(document, documentContent);

            // Prevent Original
            return false;
        }
    }
}
