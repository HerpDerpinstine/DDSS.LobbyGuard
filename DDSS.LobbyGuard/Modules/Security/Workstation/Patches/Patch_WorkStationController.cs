using DDSS_LobbyGuard.Modules.Security.Workstation.Internal;
using DDSS_LobbyGuard.SecurityExtension;
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
using Il2CppProps.WorkStation.Mouse;
using Il2CppProps.WorkStation.Scripts;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Workstation.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_WorkStationController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.WatchForStats))]
        private static void WatchForStats()
        {
            MelonDebug.Msg("WatchForStats");
        }

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
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdStartWatchForStats__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdStartWatchForStats__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Get WorkStationController
            WorkStationController controller = __0.TryCast<WorkStationController>();
            if (controller == null)
                return false;

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller.computerController, sender))
                return false;

            // Run Game Command
            controller.UserCode_CmdStartWatchForStats__NetworkIdentity__NetworkConnectionToClient(sender, __2);

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
            if (!Extras.MoreWorkstationSettings.ModuleConfig.Instance.AllowVirusesWhileServerIsDown.Value
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
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, station))
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get InfectedUsb
            FloppyDiskController usb = collectible.TryCast<FloppyDiskController>();
            if ((usb == null)
                || !usb.NetworkisInfected)
                return false;

            // Run Game Command
            station.UserCode_CmdSetVirus__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdUpdate__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdUpdate__NetworkIdentity__NetworkConnectionToClient_Prefix(
            NetworkBehaviour __0,
            NetworkConnectionToClient __2)
        {
            // Check Server Status
            if (!ServerController.connectionsEnabled)
                return false;

            // Get WorkStationController
            WorkStationController station = __0.TryCast<WorkStationController>();
            if ((station == null)
                || station.WasCollected)
                return false;

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, station))
                return false;

            // Validate Sender
            PlayerController controller = __2.identity.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            if (!Extras.MoreRoleSettings.ModuleConfig.Instance.AllowJanitorsToUpdateComputers.Value
                && lobbyPlayer.IsJanitor())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if (collectible == null)
                return false;

            // Get FloppyDiskController
            FloppyDiskController usb = collectible.TryCast<FloppyDiskController>();
            if (usb == null)
                return false;

            // Run Game Command
            bool isVirus = usb.NetworkisInfected;
            if (isVirus)
                station.UserCode_CmdSetVirus__NetworkIdentity__NetworkConnectionToClient(sender, __2);
            else
                station.UserCode_CmdUpdate__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdEnableJelly__NetworkIdentity__Boolean__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdEnableJelly__NetworkIdentity__Boolean__NetworkConnectionToClient_Prefix(
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
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, station))
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
                || !InteractionSecurity.IsPlayerWithinInteractRange(sender, station))
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
        [HarmonyPatch(typeof(WorkStationController), nameof(WorkStationController.InvokeUserCode_CmdCreateInfectedFloppyDisk__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdCreateInfectedFloppyDisk__NetworkIdentity__NetworkConnectionToClient_Prefix(
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
                || (lobbyPlayer.playerRole != PlayerRole.Slacker))
                return false;

            // Validate WorkStationController
            station = lobbyPlayer.NetworkworkStationController;
            if ((station == null)
                || station.WasCollected)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsPlayerWithinInteractRange(sender, station))
                return false;

            Usable usable = controller.GetCurrentUsable();
            if (usable == null)
                return false;

            FloppyDiskController floppy = usable.TryCast<FloppyDiskController>();
            if (floppy == null)
                return false;

            // Run Game Command
            station.UserCode_CmdCreateInfectedFloppyDisk__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            // Prevent Original
            return false;
        }
    }
}
