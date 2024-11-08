using DDSS_LobbyGuard.Components;
using DDSS_LobbyGuard.Security;
using Il2Cpp;
using Il2CppInterop.Runtime;
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

        internal static void RpcSetPlayerRoleAll(this LobbyPlayer player, PlayerRole playerRole, bool giveNewTasks)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.WriteInt((int)playerRole);
            networkWriterPooled.WriteBool(giveNewTasks);
            player.SendRPCInternal(
                InteractionSecurity.SET_PLAYERROLE_RPC_INFO.Item1,
                InteractionSecurity.SET_PLAYERROLE_RPC_INFO.Item2,
                networkWriterPooled, 0, true);
            NetworkWriterPool.Return(networkWriterPooled);
        }

        internal static void RpcSetPlayerRoleSpecific(this LobbyPlayer player, NetworkConnectionToClient receipient, PlayerRole playerRole, bool giveNewTasks)
        {
            NetworkWriterPooled networkWriterPooled = NetworkWriterPool.Get();
            networkWriterPooled.WriteInt((int)playerRole);
            networkWriterPooled.WriteBool(giveNewTasks);
            player.SendTargetRPCInternal(
                receipient,
                InteractionSecurity.SET_PLAYERROLE_RPC_INFO.Item1,
                InteractionSecurity.SET_PLAYERROLE_RPC_INFO.Item2,
                networkWriterPooled, 0);
            NetworkWriterPool.Return(networkWriterPooled);
        }
    }
}