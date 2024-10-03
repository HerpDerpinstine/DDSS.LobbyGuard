using DDSS_LobbyGuard.Security;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppProps.WorkStation.InfectedUSB;

namespace DDSS_LobbyGuard.Patches
{
    internal class Patch_WorkStationController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdEnableJelly__NetworkIdentity__Boolean))]
        private static bool InvokeUserCode_CmdEnableJelly__NetworkIdentity__Boolean_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get Values
            __1.ReadNetworkIdentity();
            bool enabled = __1.ReadBool();

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Run Game Command
            station.UserCode_CmdEnableJelly__NetworkIdentity__Boolean(sender, enabled);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdPickUpCigarettePack__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPickUpCigarettePack__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Get CigarettePack from Prefab
            CigarettePack prefabCig = station.cigarettePackPrefab.GetComponentInChildren<CigarettePack>();
            if (prefabCig == null)
                return false;

            // Get CigarettePack Interactable Name
            string interactableName = prefabCig.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName, 
                InteractionSecurity.MAX_CIG_PACKS))
                return false;

            // Run Game Command
            station.UserCode_CmdPickUpCigarettePack__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdPickUpInfectedUsb__NetworkIdentity))]
        private static bool InvokeUserCode_CmdPickUpInfectedUsb__NetworkIdentity_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Get InfectedUsb from Prefab
            InfectedUsb prefabUsb = station.cigarettePackPrefab.GetComponentInChildren<InfectedUsb>();
            if (prefabUsb == null)
                return false;

            // Get InfectedUsb Interactable Name
            string interactableName = prefabUsb.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                InteractionSecurity.MAX_INFECTED_USBS))
                return false;

            // Run Game Command
            station.UserCode_CmdPickUpInfectedUsb__NetworkIdentity(sender);

            // Prevent Original
            return false;
        }
    }
}
