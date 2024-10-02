using Il2CppMirror;
using Il2CppProps.ServerRack;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class ServerSecurity
    {
        private const float CONNECTION_DELAY = 6f;
        private static Dictionary<ServerController, Coroutine> _setConnectionCoroutines = new();

        internal static void OnSceneLoad()
        {
            // Clear Cached Coroutines
            _setConnectionCoroutines.Clear();
        }

        internal static void OnSetConnectionEnd(ServerController controller)
        {
            // Clear Cached Coroutine
            if (_setConnectionCoroutines.ContainsKey(controller))
                _setConnectionCoroutines.Remove(controller);
        }

        internal static void OnSetConnectionBegin(
            NetworkIdentity sender,
            ServerController controller, 
            bool state)
        {
            // Check for Already Running Coroutine
            if (_setConnectionCoroutines.ContainsKey(controller))
                controller.StopCoroutine(_setConnectionCoroutines[controller]);

            // Cache New Coroutine
            _setConnectionCoroutines[controller] =
                controller.StartCoroutine(
                    controller.DelayedSetConnection(sender, state, state ? 0f : CONNECTION_DELAY));
        }
    }
}
