using DDSS_LobbyGuard.Components;
using Il2Cpp;
using Il2CppMirror;
using Il2CppPlayer;
using Il2CppPlayer.Lobby;
using Il2CppProps.Scripts;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DDSS_LobbyGuard.Utils
{
    internal static class Extensions
    {
        private static Regex _rtRegex = new Regex("<.*?>", RegexOptions.Compiled);

        internal static string RemoveRichText(this string val)
            => _rtRegex.Replace(val, string.Empty);

        internal static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new ManagedEnumerator(enumerator).Pointer));

        internal static string GetUserName(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
                return null;

            return controller.NetworklobbyPlayer.username;
        }

        internal static Collectible GetCurrentCollectible(this NetworkIdentity player)
        {
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller == null)
                return null;
            Usable usable = controller.GetCurrentUsable();
            if (usable == null)
                return null;
            return usable.TryCast<Collectible>();
        }

        internal static bool IsCabinetOrganized(this FilingCabinetController cabinet)
        {
            bool isAllOrganized = false;
            foreach (DrawerController drawerController in cabinet.drawers)
            {
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

        internal static void CustomRpcSetPlayerRole(this LobbyPlayer player, PlayerRole playerRole, bool giveNewTasks, NetworkConnectionToClient receipient = null)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();

            networkWriterPooled.WriteInt((int)playerRole);
            networkWriterPooled.WriteBool(giveNewTasks);

            var rpcInfo = RPCHelper.Get(RPCHelper.eType.LobbyPlayer_RpcSetPlayerRole);
            if (receipient == null)
                player.SendRPCInternal(rpcInfo.Item1, rpcInfo.Item2, networkWriterPooled, 0, true);
            else
                player.SendTargetRPCInternal(receipient, rpcInfo.Item1, rpcInfo.Item2, networkWriterPooled, 0);

            NetworkWriterPool.Return(networkWriterPooled);
        }

        internal static LobbyPlayer GetLobbyPlayerFromConnection(this NetworkConnectionToClient connection)
        {
            foreach (NetworkIdentity networkIdentity in LobbyManager.instance.connectedLobbyPlayers)
            {
                LobbyPlayer player = networkIdentity.GetComponent<LobbyPlayer>();
                if (player.connectionToClient == connection)
                    return player;
            }
            return null;
        }
    }
}