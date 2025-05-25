using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;
using Il2CppProps.Smoking;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CigarettePack
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CigarettePack), nameof(CigarettePack.InvokeUserCode_CmdSmokeCigarette__NetworkIdentity__NetworkConnectionToClient))]
        private static bool InvokeUserCode_CmdSmokeCigarette__NetworkIdentity__NetworkConnectionToClient_Prefix(
             NetworkBehaviour __0,
             NetworkConnectionToClient __2)
        {
            // Get CigarettePack
            CigarettePack cigarettePack = __0.TryCast<CigarettePack>();

            // Get Sender
            NetworkIdentity sender = __2.identity;

            // Validate Role
            LobbyPlayer player = sender.GetComponent<LobbyPlayer>();
            if (sender.IsGhost())
                return false;

            // Validate Placement
            Collectible collectible = sender.GetCurrentCollectible();
            if ((collectible == null)
                || (collectible.GetIl2CppType() != Il2CppType.Of<CigarettePack>()))
                return false;

            // Get CigarettePack
            cigarettePack = collectible.TryCast<CigarettePack>();
            if (cigarettePack == null)
                return false;

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(sender.transform.position, cigarettePack.transform.position))
                return false;

            // Get Cigarette from Prefab
            Cigarette prefabCig = cigarettePack.cigarettePrefab.GetComponentInChildren<Cigarette>();
            if (prefabCig == null)
                return false;

            // Get Cigarette Interactable Name
            string interactableName = prefabCig.interactableName;
            if (string.IsNullOrEmpty(interactableName)
                || string.IsNullOrWhiteSpace(interactableName))
                return false;

            // Validate Count
            if (!InteractionSecurity.CanSpawnItem(interactableName,
                InteractionSecurity.MAX_CIG_PACKS))
                return false;

            // Run Game Command
            cigarettePack.UserCode_CmdStopUse__NetworkIdentity__NetworkConnectionToClient(sender, __2);

            GameObject gameObject = GameObject.Instantiate(cigarettePack.cigarettePrefab, cigarettePack.transform.position, cigarettePack.transform.rotation);
            NetworkServer.Spawn(gameObject, sender.connectionToClient);

            Usable usable = gameObject.GetComponent<Usable>();
            usable.ServerUseNoTypeVerification(sender);

            // Prevent Original
            return false;
        }
    }
}