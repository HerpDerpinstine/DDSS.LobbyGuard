﻿using DDSS_LobbyGuard.Config;
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
            while (_randomCoroutines.ContainsKey(controller))
            {
                if (ConfigHandler.Gameplay.SlackerServerOutageResetsRandomOutage.Value
                    && !ServerController.connectionsEnabled)
                    yield return null;
                else
                {
                    int secondsCount = 0;
                    int delaySeconds = Random.Range(ConfigHandler.Gameplay.RandomServerOutageDelayMin.Value, ConfigHandler.Gameplay.RandomServerOutageDelayMax.Value);
                    while (_randomCoroutines.ContainsKey(controller)
                        && (secondsCount < delaySeconds)
                        && (!ConfigHandler.Gameplay.SlackerServerOutageResetsRandomOutage.Value 
                            || ServerController.connectionsEnabled))
                    {
                        yield return new WaitForSeconds(1f);
                        secondsCount++;
                    }

                    if (ServerController.connectionsEnabled)
                    {
                        ServerController.connectionsEnabled = false;
                        controller.RpcSetConnectionEnabled(null, false);
                        yield return null;
                    }
                }
            }

            yield break;
        }

        internal static void OnSetConnection(
            NetworkIdentity sender,
            ServerController controller,
            bool state)
        {
            if (_setConnectionCoroutines.ContainsKey(controller))
                return;
            
            if (ConfigHandler.Gameplay.SlackerServerOutageResetsRandomOutage.Value
                && _randomCoroutines.ContainsKey(controller))
            {
                controller.StopCoroutine(_randomCoroutines[controller]);
                _randomCoroutines.Remove(controller);
            }

            // Cache New Coroutine
            _setConnectionCoroutines[controller] =
                controller.StartCoroutine(
                    DelayedSetConnection(controller, sender, state, state ? 0f : ConfigHandler.Gameplay.SlackerServerOutageDelay.Value));
        }

        private static IEnumerator DelayedSetConnection(ServerController controller, NetworkIdentity sender, bool state, float delay)
        {
            yield return new WaitForSeconds(delay);

            ServerController.connectionsEnabled = state;
            controller.RpcSetConnectionEnabled(sender, state);

            _setConnectionCoroutines.Remove(controller);

            if (ConfigHandler.Gameplay.SlackerServerOutageResetsRandomOutage.Value)
                OnStart(controller);

            yield break;
        }
    }
}
