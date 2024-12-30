using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
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
            if (!controller.mainServer)
                return;

            if (_randomCoroutines.ContainsKey(controller))
                controller.StopCoroutine(_randomCoroutines[controller]);

            _randomCoroutines[controller] =
                controller.StartCoroutine(RandomOutageCoroutine(controller));
        }

        private static IEnumerator RandomOutageCoroutine(ServerController controller)
        {
            while ((controller != null)
                && !controller.WasCollected
                && _randomCoroutines.ContainsKey(controller))
            {
                if ((GameManager.instance.currentGameState > (int)GameStates.WaitingForPlayerConnections)
                    && (!ConfigHandler.Gameplay.ServerOutageResetsRandomOutageTimer.Value
                        || ServerController.connectionsEnabled))
                {
                    int userMin = ConfigHandler.Gameplay.RandomServerOutageDelayMin.Value;
                    int userMax = ConfigHandler.Gameplay.RandomServerOutageDelayMax.Value;
                    if (userMax < userMin)
                    {
                        int origMin = userMin;
                        userMin = userMax;
                        userMax = origMin;
                    }

                    int secondsCount = 0;
                    int delaySeconds = Random.Range(userMin, userMax);
                    while ((controller != null)
                        && !controller.WasCollected
                        && _randomCoroutines.ContainsKey(controller)
                        && (secondsCount < delaySeconds)
                        && (!ConfigHandler.Gameplay.ServerOutageResetsRandomOutageTimer.Value
                            || ServerController.connectionsEnabled))
                    {
                        yield return new WaitForSeconds(1f);
                        secondsCount++;
                    }

                    if ((controller != null)
                        && !controller.WasCollected
                        && ServerController.connectionsEnabled)
                    {
                        ServerController.connectionsEnabled = false;
                        controller.RpcSetConnectionEnabled(null, false);
                    }
                }

			    yield return null;
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
            
            if (ConfigHandler.Gameplay.ServerOutageResetsRandomOutageTimer.Value
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
            int secondsCount = 0;
            while ((controller != null)
                && !controller.WasCollected
                && (secondsCount < delay))
            {
                yield return new WaitForSeconds(1f);
                secondsCount++;
            }

            _setConnectionCoroutines.Remove(controller);

            if ((controller != null)
                && !controller.WasCollected)
            {
                ServerController.connectionsEnabled = state;
                controller.RpcSetConnectionEnabled(sender, state);

                if (ConfigHandler.Gameplay.ServerOutageResetsRandomOutageTimer.Value)
                    OnStart(controller);
            }

            yield break;
        }
    }
}
