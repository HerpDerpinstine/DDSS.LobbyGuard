using DDSS_LobbyGuard.Security;
using DDSS_LobbyGuard.Utils;
using HarmonyLib;
using Il2CppComputer.Scripts.System;
using Il2CppMirror;
using Il2CppObjects.Scripts;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppSystem.FileSystem;
using UnityEngine;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_ComputerController
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdClick__Vector3__Int32))]
        private static bool InvokeUserCode_CmdClick__Vector3__Int32_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Validate Position
            Vector3 maxPosition = controller.canvas.sizeDelta / 2f;
            Vector3 minPosition = -maxPosition;
            Vector3 mousePos = __1.SafeReadVector3();
            mousePos.x = Mathf.Clamp(mousePos.x, minPosition.x, maxPosition.x);
            mousePos.y = Mathf.Clamp(mousePos.y, minPosition.y, maxPosition.y);
            mousePos.z = 0f;
            controller.cursor.transform.localPosition = mousePos;

            // Validate Button
            int buttonId = __1.SafeReadInt();
            if ((buttonId < 0)
                || (buttonId > 1))
                return false;

            // Run Game Command
            controller.UserCode_CmdClick__Vector3__Int32(mousePos, buttonId);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdCursorUp__Vector3))]
        private static bool InvokeUserCode_CmdCursorUp__Vector3_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Validate Position
            Vector3 maxPosition = controller.canvas.sizeDelta / 2f;
            Vector3 minPosition = -maxPosition;
            Vector3 mousePos = __1.SafeReadVector3();
            mousePos.x = Mathf.Clamp(mousePos.x, minPosition.x, maxPosition.x);
            mousePos.y = Mathf.Clamp(mousePos.y, minPosition.y, maxPosition.y);
            mousePos.z = 0f;
            controller.cursor.transform.localPosition = mousePos;

            // Run Game Command
            controller.UserCode_CmdCursorUp__Vector3(mousePos);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdSyncCursor__Vector3))]
        private static bool InvokeUserCode_CmdSyncCursor__Vector3_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Validate Position
            Vector3 maxPosition = controller.canvas.sizeDelta / 2f;
            Vector3 minPosition = -maxPosition;
            Vector3 mousePos = __1.SafeReadVector3();
            mousePos.x = Mathf.Clamp(mousePos.x, minPosition.x, maxPosition.x);
            mousePos.y = Mathf.Clamp(mousePos.y, minPosition.y, maxPosition.y);
            mousePos.z = 0f;
            controller.cursor.transform.localPosition = mousePos;

            // Run Game Command
            controller.UserCode_CmdSyncCursor__Vector3(mousePos);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdKeyPressed__String))]
        private static bool InvokeUserCode_CmdKeyPressed__String_Prefix(
            NetworkBehaviour __0,
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected
                || sender.IsGhost())
                return false;

            // Get ComputerController
            ComputerController controller = __0.TryCast<ComputerController>();

            // Validate Player
            if (!ComputerSecurity.ValidatePlayer(controller, sender))
                return false;

            // Read String
            string keyCodeStr = __1.SafeReadString();
            if (keyCodeStr == null)
                return false;

            // Validate String Length
            int keyCodeStrLen = keyCodeStr.Length;
            if ((keyCodeStrLen < 1)
                || (keyCodeStrLen > 1))
                return false;

            // Run Game Command
            controller.UserCode_CmdKeyPressed__String(keyCodeStr);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdCreateFile__String__String))]
        private static bool InvokeUserCode_CmdCreateFile__String__String_Prefix(
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            PlayerController playercontroller = sender.GetComponent<PlayerController>();
            if ((playercontroller == null)
                || playercontroller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = playercontroller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Get Workstation
            WorkStationController workstation = lobbyPlayer.workStationController;
            if ((workstation == null)
                || workstation.WasCollected)
                return false;

            // Get ComputerController
            ComputerController controller = workstation.computerController;
            if ((controller == null)
                || controller.WasCollected)
                return false;

            FileSystemManager fileIO = controller.GetComponent<FileSystemManager>();
            if ((fileIO == null)
                || fileIO.WasCollected)
                return false;

            // Override Path, Unused Anyways
            string path = "User/Desktop";

            // Get FileName
            string fileName = __1.SafeReadString();
            if (string.IsNullOrEmpty(fileName)
                || string.IsNullOrWhiteSpace(fileName))
                return false;

            FsObject file = fileIO.FindFsObject($"{path}/{fileName}");
            if ((file != null)
                && !file.WasCollected)
                return false;

            TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
            if ((textAsset == null)
                || textAsset.WasCollected)
                return false;

            controller.UserCode_CmdCreateFile__String__String(fileName, path);

            // Prevent Original
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ComputerController), nameof(ComputerController.InvokeUserCode_CmdRemoveFile__String__String))]
        private static bool InvokeUserCode_CmdRemoveFile__String__String_Prefix(
            NetworkReader __1,
            NetworkConnectionToClient __2)
        {
            // Get Sender
            NetworkIdentity sender = __2.identity;
            if ((sender == null)
                || sender.WasCollected)
                return false;

            PlayerController playercontroller = sender.GetComponent<PlayerController>();
            if ((playercontroller == null)
                || playercontroller.WasCollected)
                return false;

            LobbyPlayer lobbyPlayer = playercontroller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected
                || lobbyPlayer.IsGhost())
                return false;

            // Get Workstation
            WorkStationController workstation = lobbyPlayer.workStationController;
            if ((workstation == null)
                || workstation.WasCollected)
                return false;

            // Get ComputerController
            ComputerController controller = workstation.computerController;
            if ((controller == null)
                || controller.WasCollected)
                return false;

            FileSystemManager fileIO = controller.GetComponent<FileSystemManager>();
            if ((fileIO == null)
                || fileIO.WasCollected)
                return false;

            // Override Path, Unused Anyways
            string path = "User/Desktop";

            // Get FileName
            string fileName = __1.SafeReadString();
            if (string.IsNullOrEmpty(fileName)
                || string.IsNullOrWhiteSpace(fileName))
                return false;

            FsObject file = fileIO.FindFsObject($"{path}/{fileName}");
            if ((file == null)
                || file.WasCollected)
                return false;

            TextAsset textAsset = Resources.Load<TextAsset>("files/" + fileName);
            if ((textAsset == null)
                || textAsset.WasCollected)
                return false;

            controller.UserCode_CmdRemoveFile__String__String(fileName, path);

            // Prevent Original
            return false;
        }
    }
}
