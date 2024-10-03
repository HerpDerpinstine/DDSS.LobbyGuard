using DDSS_LobbyGuard.Components;
using Il2Cpp;
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

        internal static Collectible GetCurrentCollectible(this LobbyPlayer player)
        {
            PlayerController controller = player.NetworkplayerController.GetComponent<PlayerController>();
            if (controller == null)
                return null;
            Usable usable = controller.GetCurrentUsable();
            if (usable == null)
                return null;
            return usable.TryCast<Collectible>();
        }

        internal static Coroutine StartCoroutine<T>(this T behaviour, IEnumerator enumerator)
            where T : MonoBehaviour
            => behaviour.StartCoroutine(
                new Il2CppSystem.Collections.IEnumerator(
                new ManagedEnumerator(enumerator).Pointer));
    }
}