using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;

namespace DDSS_LobbyGuard.Modules.Security.Fish.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_FishTankController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FishTankController), nameof(FishTankController.UserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient_Prefix(NetworkIdentity __0,
            NetworkConnectionToClient __2)
        {
            // Get FishTankController
            FishTankController tank = __0.TryCast<FishTankController>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            PlayerController controller = sender.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost()
                || !InteractionSecurity.IsWithinRange(sender.transform.position, tank.transform.position))
                return false;

            if (!Extras.MoreRoleSettings.ModuleConfig.Instance.AllowJanitorsToPourInk.Value
                && lobbyPlayer.IsJanitor())
                return false;

            if (!Extras.MoreRoleSettings.ModuleConfig.Instance.AllowSpecialistsToPourInk.Value
                && lobbyPlayer.IsSpecialist())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || collectible.WasCollected)
                return false;

            // Validate InkCartridge
            InkCartridge ink = collectible.TryCast<InkCartridge>();
            if ((ink == null)
                || ink.WasCollected)
                return false;

            tank.UserCode_CmdPourInk__NetworkIdentity__NetworkConnectionToClient(sender, __2);


            // Prevent Original
            return false;
        }
    }
}
