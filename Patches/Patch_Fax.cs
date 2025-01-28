using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Printer;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Fax
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Fax), nameof(Fax.InvokeUserCode_CmdFax__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdFax__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get Fax
            Fax fax = __0.TryCast<Fax>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, fax.transform.position))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || ((collectible.GetIl2CppType() != Il2CppType.Of<Document>())
                    && (collectible.GetIl2CppType() != Il2CppType.Of<PrintedImage>())))
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
