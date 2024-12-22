using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;
using System.Linq;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Binder
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.UserCode_CmdAddDocumentServer__String__String))]
        private static bool UserCode_CmdAddDocumentServer__String__String_Prefix(Binder __instance, string __0, string __1)
        {
            // Manually Run RPC on Server
            __instance.UserCode_RpcAddDocument__String__String(__0, __1);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.UserCode_CmdAddDocument__Document__NetworkIdentity__NetworkConnectionToClient))]
        private static bool UserCode_CmdAddDocument__Document__NetworkIdentity_Prefix(Binder __instance, Document __0, NetworkIdentity __1)
        {
            // Manually Run RPC on Server
            __instance.UserCode_RpcAddDocument__String__String(__0.interactableName, __0.text);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.UserCode_CmdGrabDocument__String__String__NetworkIdentity__NetworkConnectionToClient))]
        private static bool UserCode_CmdGrabDocument__String__String__NetworkIdentity__NetworkConnectionToClient_Prefix(Binder __instance, 
            string __0, 
            string __1,
            NetworkIdentity __2)
        {
            // Manually Run RPC on Server
            __instance.UserCode_RpcGrabDocument__String__String__NetworkIdentity(__0, __1, __2);

            // Run Original
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.InvokeUserCode_CmdAddDocument__Document__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdAddDocument__Document__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Binder
            Binder binder = __0.TryCast<Binder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Drop
            if (!InteractionSecurity.CanStopUseUsable(sender, false))
                return false;

            // Validate Count
            if (binder.documents.Count >= InteractionSecurity.MAX_DOCUMENTS_BINDER)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, binder.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected
                || ((collectible.GetIl2CppType() != Il2CppType.Of<Document>())
                    && (collectible.GetIl2CppType() != Il2CppType.Of<PrintedImage>())))
                return false;

            // Get Document
            Document doc = collectible.TryCast<Document>();
            if ((doc == null)
                || doc.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(binder.transform.position, doc.transform.position))
                return false;

            // Run Game Command
            binder.UserCode_CmdAddDocument__Document__NetworkIdentity__NetworkConnectionToClient(doc, sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.InvokeUserCode_CmdAddDocumentServer__String__String))]
        private static bool InvokeUserCode_CmdAddDocumentServer__String__String_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check for Server
            if (__2.identity.isServer)
                return true;

            // Get Binder
            Binder binder = __0.TryCast<Binder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Drop
            if (!InteractionSecurity.CanStopUseUsable(sender, false))
                return false;

            // Validate Count
            if (binder.documents.Count >= InteractionSecurity.MAX_DOCUMENTS_BINDER)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, binder.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected
                || ((collectible.GetIl2CppType() != Il2CppType.Of<Document>())
                    && (collectible.GetIl2CppType() != Il2CppType.Of<PrintedImage>())))
                return false;

            // Get Document
            Document doc = collectible.TryCast<Document>();
            if ((doc == null)
                || doc.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(binder.transform.position, doc.transform.position))
                return false;

            // Run Game Command
            binder.UserCode_CmdAddDocumentServer__String__String(doc.interactableName, doc.text);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Binder), nameof(Binder.InvokeUserCode_CmdGrabDocument__String__String__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdGrabDocument__String__String__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Binder
            Binder binder = __0.TryCast<Binder>();

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Count
            if (binder.documents.Count <= 0)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, binder.transform.position))
                return false;

            // Get Collectible
            Collectible collectible = binder.documentPrefab.GetComponent<Collectible>();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, collectible))
                return false;

            // Get Document Name
            string documentName = __1.SafeReadString();
            if (string.IsNullOrEmpty(documentName)
                || string.IsNullOrWhiteSpace(documentName))
                return false;

            // Get Document from List
            Il2CppSystem.ValueTuple<string, string, InteractionAlternative>[] docs = binder.documents.ToArray();
            Il2CppSystem.ValueTuple<string, string, InteractionAlternative> doc = docs.FirstOrDefault(x => x.Item1 == documentName);
            if ((doc == null)
                || doc.WasCollected)
                return false;

            // Run Game Command
            binder.UserCode_CmdGrabDocument__String__String__NetworkIdentity__NetworkConnectionToClient(doc.Item1, doc.Item2, sender, __2);

            // Prevent Original
            return false;
        }
    }
}
