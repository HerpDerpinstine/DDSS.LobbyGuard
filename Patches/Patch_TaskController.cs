using HarmonyLib;
using Il2CppPlayer.Tasks;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_TaskController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TaskController), nameof(TaskController.InvokeUserCode_CmdInstantiateAndAssignSecretObjective__Int32__NetworkIdentity__NetworkIdentity))]
        private static bool InvokeUserCode_CmdInstantiateAndAssignSecretObjective__Int32__NetworkIdentity__NetworkIdentity_Prefix()
        {
            // Prevent Original
            return false;
        }
    }
}
