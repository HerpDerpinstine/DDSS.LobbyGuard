using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Paper.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Fax
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Il2Cpp.Fax), nameof(Il2Cpp.Fax.InvokeUserCode_CmdFax__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFax__NetworkIdentity__NetworkConnectionToClient_Prefix(
             NetworkBehaviour __0,
             NetworkConnectionToClient __2)
        {
            // Get Fax
            Il2Cpp.Fax fax = __0.TryCast<Il2Cpp.Fax>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, fax.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null
                || collectible.GetIl2CppType() != Il2CppType.Of<Document>()
                    && collectible.GetIl2CppType() != Il2CppType.Of<PrintedImage>())
                return false;

            // Get Document
            Document doc = collectible.TryCast<Document>();
            if (doc == null)
                return false;

            // Run Game Command
            fax.UserCode_CmdFax__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}