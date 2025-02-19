using DDSS_LobbyGuard.Components;
using Il2Cpp;
using Il2CppComputer.Scripts.System;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppPlayer.Scripts;
using Il2CppPlayer.StateMachineLogic;
using Il2CppProps.Scripts;
using Il2CppProps.WorkStation.Phone;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DDSS_LobbyGuard.Utils
{
    public static class Extensions
    {
        private static Regex _rtRegex = new Regex("<[^>]*>");

        public static string RemoveRichText(this string val)
            => _rtRegex.Replace(val, string.Empty);

        public static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new ManagedEnumerator(enumerator).Pointer));

        public static string GetUserName(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return null;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return null;

            return lobbyPlayer.username;
        }

        public static bool IsGhost(this LobbyPlayer player)
            => (player.playerRole == PlayerRole.None);
        public static bool IsGhost(this PlayerController controller)
        {
            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return true;

            return lobbyPlayer.IsGhost();
        }
        public static bool IsGhost(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return true;

            return false;
        }

        public static bool IsJanitor(this LobbyPlayer player)
            => (player.playerRole == PlayerRole.Janitor);
        public static bool IsJanitor(this PlayerController controller)
        {
            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return false;

            return lobbyPlayer.IsJanitor();
        }
        public static bool IsJanitor(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            return controller.IsJanitor();
        }

        public static bool IsSpecialist(this LobbyPlayer player)
            => (player.playerRole == PlayerRole.Specialist);
        public static bool IsSpecialist(this PlayerController controller)
        {
            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return false;

            return lobbyPlayer.IsSpecialist();
        }
        public static bool IsSpecialist(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return false;

            return controller.IsSpecialist();
        }

        public static PlayerRole GetPlayerRole(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return PlayerRole.None;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return PlayerRole.None;

            return lobbyPlayer.playerRole;
        }

        public static SubRole GetPlayerSubRole(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return SubRole.None;

            LobbyPlayer lobbyPlayer = controller.NetworklobbyPlayer;
            if ((lobbyPlayer == null)
                || lobbyPlayer.WasCollected)
                return SubRole.None;

            return lobbyPlayer.subRole;
        }

        public static LobbyPlayer GetLobbyPlayer(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return null;

             return controller.NetworklobbyPlayer;
        }

        public static Collectible GetCurrentCollectible(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if ((controller == null)
                || controller.WasCollected)
                return null;

            Usable usable = controller.GetCurrentUsable();
            if ((usable == null)
                || usable.WasCollected)
                return null;

            return usable.TryCast<Collectible>();
        }

        public static Collectible GetCurrentCollectible(this PlayerController player)
        {
            Usable usable = player.GetCurrentUsable();
            if ((usable == null)    
                || usable.WasCollected)
                return null;

            return usable.TryCast<Collectible>();
        }

        public static bool IsPointing(this PlayerController player)
            => (player.NetworktargetUBState == (int)UpperBodyStates.Pointing);

        public static bool IsRaisingHand(this PlayerController player)
            => (player.NetworktargetUBState == (int)UpperBodyStates.RaiseHand);

        public static bool IsClapping(this PlayerController player)
            => (player.NetworktargetUBState == (int)UpperBodyStates.Clapping);

        public static bool IsWaving(this PlayerController player)
            => (player.NetworktargetUBState == (int)UpperBodyStates.Waving);

        public static bool IsFacePalming(this PlayerController player)
            => (player.NetworktargetUBState == (int)UpperBodyStates.FacePalm);

        public static bool IsHandShaking(this PlayerController player)
            => ((player.NetworktargetUBState == (int)UpperBodyStates.RequestHandShake)
                || (player.NetworktargetUBState == (int)UpperBodyStates.PerformHandShake));

        public static bool IsEmoting(this PlayerController player)
            => ((player.NetworktargetState == (int)States.Dancing)
                || (player.NetworktargetState == (int)States.Humping)
                || (player.NetworktargetState == (int)States.Beg)
                || (player.NetworktargetUBState == (int)UpperBodyStates.PoundChest)
                || (player.NetworktargetUBState == (int)UpperBodyStates.Waving)
                || (player.NetworktargetUBState == (int)UpperBodyStates.Clapping)
                || (player.NetworktargetUBState == (int)UpperBodyStates.FacePalm));

        public static bool IsCabinetOrganized(this FilingCabinetController cabinet)
        {
            bool isAllOrganized = false;
            foreach (DrawerController drawerController in cabinet.drawers)
            {
                if ((drawerController == null)
                    || drawerController.WasCollected)
                    continue;

                if (!drawerController.NetworkisOrganized)
                {
                    isAllOrganized = false;
                    break;
                }
                else
                    isAllOrganized = true;
            }
            return isAllOrganized;
        }

        public static bool ServerCanShakeHand(this PlayerInteractable a)
            => a.playerController != null
                && a.NetworkplayerRequestedHandshakeWithThisPlayer == null
                && a.NetworkthisPlayerRequestedHandshakeWith == null;

        public static bool HasPlayerRequestedHandShake(this PlayerInteractable a, PlayerInteractable b)
            => a.playerController != null
                && a.playerController.netIdentity == b.NetworkplayerRequestedHandshakeWithThisPlayer;

        public static void CustomRpcSetPlayerRole(this LobbyPlayer player, PlayerRole playerRole, bool giveNewTasks, NetworkConnectionToClient receipient = null)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();

            networkWriterPooled.WriteInt((int)playerRole);
            networkWriterPooled.WriteBool(giveNewTasks);

            networkWriterPooled.SendCustomRPC(player,
                RPCHelper.eType.LobbyPlayer_RpcSetPlayerRole,
                receipient);
            NetworkWriterPool.Return(networkWriterPooled);
        }

        public static void CustomRpcClick(this ComputerController computer, Vector3 localCursorPos, int button, NetworkConnectionToClient receipient = null)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();

            networkWriterPooled.WriteVector3(localCursorPos);
            networkWriterPooled.WriteInt(button);

            networkWriterPooled.SendCustomRPC(computer,
                RPCHelper.eType.ComputerController_RpcClick,
                receipient);
            NetworkWriterPool.Return(networkWriterPooled);
        }

        public static void CustomRpcCursorUp(this ComputerController computer, Vector3 localCursorPos, NetworkConnectionToClient receipient = null)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();

            networkWriterPooled.WriteVector3(localCursorPos);

            networkWriterPooled.SendCustomRPC(computer,
                RPCHelper.eType.ComputerController_RpcCursorUp,
                receipient);
            NetworkWriterPool.Return(networkWriterPooled);
        }

        public static void SendCustomRPC<T>(this NetworkWriterPooled writer,
            T obj,
            RPCHelper.eType type,
            NetworkConnectionToClient receipient = null)
            where T : NetworkBehaviour
        {
            var rpcInfo = RPCHelper.Get(type);
            if (receipient == null)
                obj.SendRPCInternal(rpcInfo.Item1, rpcInfo.Item2, writer, 0, true);
            else
                obj.SendTargetRPCInternal(receipient, rpcInfo.Item1, rpcInfo.Item2, writer, 0);
        }

        public static LobbyPlayer GetLobbyPlayerFromConnection(this NetworkConnectionToClient connection)
        {
            if ((connection == null)
                || connection.WasCollected)
                return null;

            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.GetAllPlayers())
            {
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected)
                    continue;

                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if ((player == null)
                    || player.WasCollected)
                    continue;

                if (player.connectionToClient == connection)
                    return player;
            }
            return null;
        }

        public static List<NetworkIdentity> GetAllPlayers(this LobbyManager manager)
        {
            if ((manager == null)
                || manager.WasCollected)
                return null;

            List<NetworkIdentity> players = new();

            LobbyPlayer localPlayer = manager.GetLocalPlayer();
            if ((localPlayer != null)
                && !localPlayer.WasCollected)
                players.Add(localPlayer.netIdentity);

            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedLobbyPlayers)
            {
                if ((networkIdentity == null)
                    || networkIdentity.WasCollected
                    || players.Contains(networkIdentity))
                    continue;

                players.Add(networkIdentity);
            }

            return players;
        }

        public static void ForceCallToEnd(this PhoneController phone, NetworkIdentity sender, NetworkConnectionToClient clientConnection)
        {
            if (phone.NetworkisCallActive)
            {
                string other = phone.NetworkcallingNumber;
                if (!string.IsNullOrEmpty(other)
                    && !string.IsNullOrWhiteSpace(other))
                {
                    string self = phone.phoneNumber;
                    if (!string.IsNullOrEmpty(self)
                        && !string.IsNullOrWhiteSpace(self))
                    {
                        PhoneManager.instance.UserCode_CmdEndCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, self, other, clientConnection);
                    }
                }
            }

            if (phone.NetworkisDialing)
            {
                string reciever = phone.phoneNumber;
                if (!string.IsNullOrEmpty(reciever)
                    && !string.IsNullOrWhiteSpace(reciever))
                {
                    string caller = phone.NetworkcallingNumber;
                    if (!string.IsNullOrEmpty(caller)
                        && !string.IsNullOrWhiteSpace(caller))
                    {
                        if (PhoneManager.instance.allNpcPhoneNumbers.ContainsKey(caller))
                        {
                            if (!PhoneManager.instance.allNpcPhoneNumbers.ContainsKey(reciever))
                                PhoneManager.instance.UserCode_CmdCancelCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, reciever, reciever, clientConnection);
                        }
                        else
                        {
                            if (PhoneManager.instance.allNpcPhoneNumbers.ContainsKey(reciever))
                            {
                                if (!PhoneManager.instance.allNpcPhoneNumbers.ContainsKey(caller))
                                    PhoneManager.instance.UserCode_CmdCancelCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, caller, caller, clientConnection);
                            }
                            else
                                PhoneManager.instance.UserCode_CmdCancelCall__NetworkIdentity__String__String__NetworkConnectionToClient(sender, reciever, caller, clientConnection);
                        }
                    }
                }
            }
        }

        public static void Shuffle<T>(this List<T> list)
        {
            int i = list.Count;
            while (i > 1)
            {
                i--;
                int num = Random.Range(0, i + 1);
                int num2 = num;
                int num3 = i;
                T t = list[i];
                T t2 = list[num];
                list[num2] = t;
                list[num3] = t2;
            }
        }
    }
}