using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class VirusSecurity
    {
        private static Dictionary<VirusController, Coroutine> _randomCoroutines = new();

        internal static void OnSceneLoad()
            => _randomCoroutines.Clear();

        internal static void OnStart(VirusController controller)
        {
            if (!NetworkServer.activeHost)
                return;

            _randomCoroutines[controller] =
                controller.StartCoroutine(RandomVirusCoroutine(controller));
        }

        private static IEnumerator RandomVirusCoroutine(VirusController controller)
        {
            while ((controller != null)
                && !controller.WasCollected
                && _randomCoroutines.ContainsKey(controller))
            {
                if ((GameManager.instance.currentGameState > (int)GameStates.StartGame)
                    && (controller.computerController.user != null)
                    && (!ConfigHandler.Gameplay.WorkStationVirusResetsRandomVirusTimer.Value
                        || !controller.isVirusActive))
                {
                    int secondsCount = 0;
                    float delaySeconds = controller.virusInfectionTimeLimit;
                    while ((controller != null)
                        && !controller.WasCollected
                        && _randomCoroutines.ContainsKey(controller)
                        && (secondsCount < delaySeconds)
                        && (!ConfigHandler.Gameplay.WorkStationVirusResetsRandomVirusTimer.Value
                            || !controller.isVirusActive))
                    {
                        yield return new WaitForSeconds(1f);
                        secondsCount++;
                        controller.virusInfectionTime += secondsCount;
                    }

                    if ((controller != null)
                        && !controller.WasCollected
                        && !controller.isVirusActive)
                    {
                        int userMin = ConfigHandler.Gameplay.RandomWorkStationVirusDelayMin.Value;
                        int userMax = ConfigHandler.Gameplay.RandomWorkStationVirusDelayMax.Value;
                        if (userMax < userMin)
                        {
                            int origMin = userMin;
                            userMin = userMax;
                            userMax = origMin;
                        }

                        controller.virusInfectionTime = 0f;
                        controller.virusInfectionTimeLimit = Random.Range(userMin, userMax);

                        if (!controller.NetworkisFirewallActive 
                            || InteractionSecurity.IsSlacker(controller.computerController.user))
                        {
                            // Get Game Rule
                            float probability = GameManager.instance.virusProbability;

                            // RandomGen
                            if (Random.Range(0f, 100f) < (probability * 100f))
                                controller.ServerSetVirus(true);
                        }
                    }
                }

			    yield return null;
            }

            if ((controller != null)
                && !controller.WasCollected
                && _randomCoroutines.ContainsKey(controller))
                _randomCoroutines.Remove(controller);

            yield break;
        }
    }
}
