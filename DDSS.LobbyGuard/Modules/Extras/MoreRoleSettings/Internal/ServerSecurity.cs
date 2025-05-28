using Il2CppMirror;
using Il2CppProps.ServerRack;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings.Internal
{
    internal static class ServerSecurity
    {
        private static Dictionary<ServerController, Coroutine> _outageCoroutines = new();

        internal static void OnSceneLoad()
        {
            // Clear Cached Coroutines
            _outageCoroutines.Clear();
        }

        internal static void OnOutageEnd(ServerController server)
        {
            // Clear Cached Coroutine
            if (_outageCoroutines.ContainsKey(server))
                _outageCoroutines.Remove(server);
        }

        internal static void OnOutageBegin(
            NetworkIdentity sender,
            ServerController server,
            bool state)
        {
            // Check for Already Running Coroutine
            if (_outageCoroutines.ContainsKey(server))
                server.StopCoroutine(_outageCoroutines[server]);

            // Cache New Coroutine
            _outageCoroutines[server] =
                server.StartCoroutine(
                    server.DelayedSetConnection(sender, state, !state ? ModuleConfig.Instance.SlackerServerOutageDelay.Value : 0f));
        }
    }
}
