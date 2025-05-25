using HarmonyLib;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
using Il2CppMirror;
using UnityEngine;
using Il2CppWindow;
using DDSS_LobbyGuard.Utils;
using Il2CppObjects.Scripts;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Patches
{
    [LobbyModulePatch(typeof(ModuleMain))]
    internal class Patch_TopBar
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TopBar), nameof(TopBar.UpdateDifference))]
        private static bool UpdateDifference_Prefix(TopBar __instance)
        {
            // Validate Drag
            if (!__instance.isDragging)
                return false;

            // Validate Window
            ApplicationWindow window = __instance._applicationWindow;
            if (window == null
                || window.WasCollected)
                return false;

            // Validate Host
            if (!NetworkServer.activeHost || !ModuleConfig.Instance.EnforceComputerWindowBoundary.Value)
                return true;

            // Get ComputerController
            ComputerController computer = __instance.GetComponentInParent<ComputerController>();
            if (computer == null
                || computer.WasCollected)
                return false;

            // Get Mouse Position and Movement Delta
            Vector3 mousePos = __instance.GetMousePos();
            Vector3 mouseStartPos = __instance.cursorStartPos;
            Vector3 mouseDelta = mousePos - mouseStartPos;

            // Get Window Position and Size Delta
            Vector3 windowPos = window.transform.localPosition;
            Vector3 windowSizeDelta = window.content.sizeDelta / 2f;

            // Get Monitor Size Delta
            Vector3 monitorSizeDelta = computer.canvas.sizeDelta / 2f;

            // Get Monitor Bounds Min Position
            Vector3 minPosition = computer.canvas.rect.min - window.content.rect.min;
            minPosition.x += 8;
            minPosition.y += 54;

            // Get Monitor Bounds Max Position
            Vector3 maxPosition = computer.canvas.rect.max - window.content.rect.max;
            maxPosition.x -= 8;
            maxPosition.y -= 84;

            // Cache New Values
            Vector3 newMousePos = mousePos;
            Vector3 newMouseDelta = mouseDelta;
            Vector3 newWindowPos = windowPos + mouseDelta;
            bool shouldMove = false;

            // Validate Window Position X
            if (newWindowPos.x < minPosition.x
                || newWindowPos.x > maxPosition.x)
            {
                // Signify Should be Fixed
                shouldMove = true;

                // Clamp Window Position
                float clampedX = Mathf.Clamp(newWindowPos.x, minPosition.x, maxPosition.x);

                // Offset Mouse Position by Clamped Window Position
                float targetDelta = newWindowPos.x - clampedX;
                newMouseDelta.x -= targetDelta;
                newMousePos.x -= targetDelta;
            }

            // Validate Window Position Y
            if (newWindowPos.y < minPosition.y
                || newWindowPos.y > maxPosition.y)
            {
                // Signify Should be Fixed
                shouldMove = true;

                // Clamp Window Position
                float clampedY = Mathf.Clamp(newWindowPos.y, minPosition.y, maxPosition.y);

                // Offset Mouse Position by Clamped Window Position
                float targetDelta = newWindowPos.y - clampedY;
                newMouseDelta.y -= targetDelta;
                newMousePos.y -= targetDelta;
            }

            // Apply Changes when Not Fixed or Not Locally Using
            if (!shouldMove || !computer.isLocalPlayerUsingComputer)
            {
                __instance.cursor.transform.localPosition = mousePos;
                __instance.cursorStartPos = mousePos;
                window.MoveWindow(mouseDelta);
            }

            // Check if Should be Fixed
            if (shouldMove)
            {
                // Check if Locally being Used
                if (computer.isLocalPlayerUsingComputer)
                {
                    // Apply Changes Locally
                    __instance.cursor.transform.localPosition = newMousePos;
                    __instance.cursorStartPos = newMousePos;
                    window.MoveWindow(newMouseDelta);
                }
                else
                {
                    // Get WorkStationController
                    WorkStationController station = computer._workStationController;

                    // Get Using Player
                    NetworkIdentity usingPlayer = station.NetworkusingPlayerController;

                    // Regrab the Window
                    //computer.CustomRpcClick(mousePos, 0, usingPlayer.connectionToClient);
                    foreach (var player in LobbyManager.instance.GetAllPlayers())
                    {
                        if (player == usingPlayer)
                            continue;
                        computer.CustomRpcClick(mouseStartPos, 0, player.connectionToClient);
                    }

                    // Move and Drop the Window
                    computer.RpcCursorUp(newMousePos);
                }
            }

            // Prevent Original
            return false;
        }
    }
}