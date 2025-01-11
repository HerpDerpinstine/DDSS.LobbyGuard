using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_PermanentMarkerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PermanentMarkerController), nameof(PermanentMarkerController.InvokeUserCode_CmdWriteOnWall__NetworkIdentity__Vector3__GameObject__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdWriteOnWall__NetworkIdentity__Vector3__GameObject__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get PermanentMarkerController
            PermanentMarkerController marker = __0.TryCast<PermanentMarkerController>();
            if (marker == null)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<PermanentMarkerController>()))
                return false;

            // Get PermanentMarkerController
            marker = collectible.TryCast<PermanentMarkerController>();
            if (marker == null)
                return false;

            // Get Position
            __1.SafeReadNetworkIdentity();
            Vector3 pos = __1.SafeReadVector3();

            // Get GameObject
            GameObject surface = __1.SafeReadGameObject();
            if (surface == null)
                return false;

            // Validate Surface Layer
            if (((marker.surfaceLayer >> surface.layer) & 1) != 1)
                return false;

            WritableSurface writeSurface = surface.GetComponent<WritableSurface>();
            if (writeSurface == null)
                return false;

            Vector3 localDif = surface.transform.InverseTransformPoint(pos);
            if ((Mathf.Abs(localDif.x) > 1f)
                || (Mathf.Abs(localDif.y) > 1f)
                || (Mathf.Abs(localDif.z) > 2f))
                return false;

            if (!InteractionSecurity.IsWithinRange(sender.transform.position, pos))
                return false;

            // Run Game Command
            marker.UserCode_CmdWriteOnWall__NetworkIdentity__Vector3__GameObject__NetworkConnectionToClient(sender, pos, surface, sender.connectionToClient);

            // Prevent Original
            return false;
        }
    }
}
