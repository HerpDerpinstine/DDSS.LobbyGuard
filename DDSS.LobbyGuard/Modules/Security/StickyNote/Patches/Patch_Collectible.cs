using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppMirror;
using Il2CppProps.Scripts;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.StickyNote.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_Collectible
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Collectible), nameof(Collectible.ServerUseNoTypeVerification))]
        private static void ServerUseNoTypeVerification_Prefix(Collectible __instance, NetworkIdentity __0)
        {
            // Get StickyNoteController
            eConfigHostType configValue = ModuleConfig.Instance.UsernamesOnStickyNotes.Value;
            if ((configValue != eConfigHostType.DISABLED)
                && (__instance.GetIl2CppType() == Il2CppType.Of<StickyNoteController>()))
            {
                // Apply New Name
                string userName = __0.GetUserName();
                if (!string.IsNullOrEmpty(userName)
                    && !string.IsNullOrWhiteSpace(userName))
                {
                    string newName = $"{userName.RemoveRichText()}'s Sticky Note";
                    if (configValue == eConfigHostType.HOST_ONLY)
                        __instance.interactableName =
                            __instance.label = newName;
                    else
                        __instance.NetworkinteractableName =
                            __instance.Networklabel = newName;
                }
            }
        }
    }
}