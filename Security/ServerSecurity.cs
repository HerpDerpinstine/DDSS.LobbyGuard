using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2CppMirror;
using Il2CppProps.ServerRack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class ServerSecurity
    {
        private static Dictionary<ServerController, Coroutine> _randomCoroutines = new();
        private static Dictionary<ServerController, Coroutine> _setConnectionCoroutines = new();

        internal static void OnSceneLoad()
        {
            _randomCoroutines.Clear();
            _setConnectionCoroutines.Clear();
        }

        internal static void OnStart(ServerController controller)
        {
            _randomCoroutines[controller] =
                controller.StartCoroutine(RandomOutageCoroutine(controller));
        }

        private static IEnumerator RandomOutageCoroutine(ServerController controller)
        {
            while (true)
            {
                if (!ServerController.connectionsEnabled)
                    yield return null;
                else
                {
                    yield return new WaitForSeconds(Random.Range(120, 600));
                    ServerController.connectionsEnabled = false;
                    controller.RpcSetConnectionEnabled(null, false);
                    yield return null;
                }
            }

            if (_randomCoroutines.ContainsKey(controller))
                _randomCoroutines.Remove(controller);

            yield break;
        }

        internal static void OnSetConnection(
            NetworkIdentity sender,
            ServerController controller,
            bool state)
        {
            if (_setConnectionCoroutines.ContainsKey(controller))
                return;
            
            if (_randomCoroutines.ContainsKey(controller))
            {
                controller.StopCoroutine(_randomCoroutines[controller]);
                _randomCoroutines.Remove(controller);
            }

            // Cache New Coroutine
            _setConnectionCoroutines[controller] =
                controller.StartCoroutine(
                    DelayedSetConnection(controller, sender, state, state ? 0f : ConfigHandler.Gameplay.SlackerServerOutageDelay.Value));
        }

        private static IEnumerator DelayedSetConnection(ServerController controller, NetworkIdentity arg0, bool state, float delay)
        {
            yield return new WaitForSeconds(delay);

            ServerController.connectionsEnabled = state;
            controller.RpcSetConnectionEnabled(arg0, state);

            _setConnectionCoroutines.Remove(controller);
            OnStart(controller);

            yield break;
        }
    }
}
