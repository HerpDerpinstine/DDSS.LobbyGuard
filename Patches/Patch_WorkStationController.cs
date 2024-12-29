using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Tasks;
using Il2CppProps.Scripts;
using Il2CppProps.ServerRack;
using Il2CppProps.WorkStation.InfectedUSB;
using Il2CppProps.WorkStation.Mouse;
using Il2CppProps.WorkStation.Scripts;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_WorkStationController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.SpawnDeskItems))]
        private static bool SpawnDeskItems_Prefix(WorkStationController __instance, PlayerRole __0)
        {
            // Check for Server
            if (!__instance.isServer)
                return false;
            if (!NetworkServer.activeHost)
                return false;

            // Spawn New Mouse
            MouseHolder mouseHolder = __instance.mouseHolder;
            if ((mouseHolder != null)
                && !mouseHolder.WasCollected)
            {
                mouseHolder.ServerDestroyAllCollectibles();
                CollectibleSecurity.SpawnMouse(__instance.mousePrefab, mouseHolder);
            }

            // Spawn New Mug
            MugHolder mugHolder = __instance.mugHolder;
            if ((mugHolder != null)
                && !mugHolder.WasCollected)
            {
                mugHolder.ServerDestroyAllCollectibles();
                CollectibleSecurity.SpawnMug(
                    (__0 == PlayerRole.Manager) ? __instance.bossMugPrefab : __instance.mugPrefab,
                    mugHolder);
            }

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();
            if ((station == null)
                || station.WasCollected
                || !station.NetworkisEmpty)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;
            if (sender.GetPlayerRole() != PlayerRole.Manager)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, station.transform.position))
                return false;

            // Get Target
            __1.SafeReadNetworkIdentity();
            NetworkIdentity targetIdentity = __1.SafeReadNetworkIdentity();
            if ((targetIdentity == null)
                || targetIdentity.WasCollected)
                return false;

            LobbyPlayer target = targetIdentity.GetComponent<LobbyPlayer>();
            if ((target == null)
                || target.WasCollected)
                return false;

            station.UserCode_CmdAssignDesk__NetworkIdentity__LobbyPlayer__NetworkConnectionToClient(sender, target, __2); 

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdSetVirus__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSetVirus__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check Server Status
            if (!ConfigHandler.Gameplay.AllowVirusesWhileServerIsDown.Value
                && !ServerController.connectionsEnabled)
                return false;

            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || !InteractionSecurity.IsSlacker(lobbyPlayer))
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get InfectedUsb
            InfectedUsb usb = collectible.TryCast<InfectedUsb>();
            if (usb == null)
                return false;

            // Run Game Command
            station.UserCode_CmdSetVirus__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

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

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Get Values
            __1.SafeReadNetworkIdentity();
            bool enabled = __1.SafeReadBool();

            // Validate Placement
            if (enabled)
            {
                Collectible collectible = sender.GetCurrentCollectible();
                if ((collectible == null)
                    || (collectible.GetIl2CppType() != Il2CppType.Of<Jelly>()))
                    return false;

                // Get Jelly
                Jelly jelly = collectible.TryCast<Jelly>();
                if (jelly == null)
                    return false;

                // Destroy Jelly
                jelly.ServerDestroyCollectible();
            }

            // Manually Trigger Task Locally
            if (sender.isLocalPlayer
                && enabled)
            {
                // Get Workstation Owner and Trigger TaskHook
                LobbyPlayer component = station.NetworkownerLobbyPlayer.GetComponent<LobbyPlayer>();
                if ((component != null)
                    && !component.WasCollected)
                    TaskHook.TriggerTaskHookCommandStatic(new TaskHook(null, "Stapler", component.Networkusername, "Put in Jelly", null, null));
            }

            // Send RPC
            station.RPCEnableJelly(enabled);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdPickUpCigarettePack__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPickUpCigarettePack__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (sender.IsGhost()
                || !InteractionSecurity.IsWithinRange(
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

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, prefabCig))
                return false;

            // Run Game Command
            station.UserCode_CmdPickUpCigarettePack__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdPickUpInfectedUsb__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPickUpInfectedUsb__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || !InteractionSecurity.IsSlacker(lobbyPlayer))
                return false;

            // Validate WorkStationController
            station = lobbyPlayer.NetworkworkStationController;
            if ((station == null)
                || station.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(
                sender.transform.position,
                station.transform.position))
                return false;

            // Get InfectedUsb from Prefab
            InfectedUsb prefabUsb = station.infectedUsbPrefab.GetComponentInChildren<InfectedUsb>();
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

            // Validate Grab
            if (!InteractionSecurity.CanUseUsable(sender, prefabUsb))
                return false;

            // Run Game Command
            station.UserCode_CmdPickUpInfectedUsb__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
