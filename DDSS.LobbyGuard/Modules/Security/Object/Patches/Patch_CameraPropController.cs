﻿using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.SecurityExtension;
using HarmonyLib;
using Il2Cpp;
using Il2CppMirror;
using Il2CppProps.CameraProp;
using MelonLoader;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Security.Object.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_CameraPropController
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraPropController), nameof(CameraPropController.OnStart))]
        private static void OnStart_Postfix(CameraPropController __instance)
        {
            if ((CameraPropController.instance != null)
                && !CameraPropController.instance.WasCollected
                && (TaskHighlighter.instance != null)
                && !TaskHighlighter.instance.WasCollected
                && (TaskHighlighter.instance.highlightObjects != null)
                && !TaskHighlighter.instance.highlightObjects.WasCollected)
            {
                foreach (HighlightObject highlighter in TaskHighlighter.instance.highlightObjects)
                {
                    if (highlighter.highlightObject != CameraPropController.instance.gameObject)
                        continue;
                    highlighter.highlightObject = __instance.gameObject;
                    break;
                }
                TaskHighlighter.instance.UpdateHighlighter();
            }
            CameraPropController.instance = __instance;

            // Validate Prefab
            if (!NetworkServer.activeHost)
                return;
            if ((CollectibleSecurity._cameraPrefab != null)
                && !CollectibleSecurity._cameraPrefab.WasCollected)
                return;

            // Temporarily Disable This Camera
            __instance.gameObject.SetActive(false);

            // Cache Starting Position and Rotation
            CollectibleSecurity._cameraSpawnPos = __instance.gameObject.transform.position;
            CollectibleSecurity._cameraSpawnRot = __instance.gameObject.transform.rotation;

            // Clone Camera into Temporary Prefab
            CollectibleSecurity._cameraPrefab = GameObject.Instantiate(__instance.gameObject, 
                CollectibleSecurity._cameraSpawnPos, 
                CollectibleSecurity._cameraSpawnRot,
                __instance.gameObject.transform.parent);
            CollectibleSecurity._cameraPrefab.name = "CameraPrefab";

            // Fix Prefab
            NetworkIdentity netIdentity = CollectibleSecurity._cameraPrefab.GetComponent<NetworkIdentity>();
            netIdentity.ResetState();

            // Add Initial CollectibleSecurityHandler to This Camera
            CollectibleSecurityHandler handler = __instance.gameObject.AddComponent<CollectibleSecurityHandler>();
            handler.collectibleType = CollectibleSecurityHandler.eCollectibleType.CAMERA;

            // Re-Enable This Camera
            CameraPropController.instance = __instance;
            __instance.gameObject.SetActive(true);
        }
    }
}
