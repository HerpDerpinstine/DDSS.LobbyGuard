using DDSS_LobbyGuard.Config;
using DDSS_LobbyGuard.SecurityExtension;
using DDSS_LobbyGuard.Utils;
using Il2Cpp;
using Il2CppGameManagement;
using Il2CppGameManagement.StateMachine;
using Il2CppMirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.Extras.MoreWorkstationSettings.Internal
{
    internal static class VirusHandler
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
            while (controller != null
                && !controller.WasCollected
                && _randomCoroutines.ContainsKey(controller))
            {
                if (GameManager.instance.currentGameState > (int)GameStates.StartGame
                    && controller.computerController.user != null
                    && (!ModuleConfig.Instance.WorkStationVirusResetsRandomVirusTimer.Value
                        || !controller.isVirusActive))
                {
                    int secondsCount = 0;
                    float delaySeconds = controller.virusInfectionTimeLimit;
                    while (controller != null
                        && !controller.WasCollected
                        && _randomCoroutines.ContainsKey(controller)
                        && secondsCount < delaySeconds
                        && (!ModuleConfig.Instance.WorkStationVirusResetsRandomVirusTimer.Value
                            || !controller.isVirusActive))
                    {
                        yield return new WaitForSeconds(1f);
                        secondsCount++;
                        controller.virusInfectionTime += secondsCount;
                    }

                    if (controller != null
                        && !controller.WasCollected
                        && !controller.isVirusActive)
                    {
                        int userMin = ModuleConfig.Instance.RandomWorkStationVirusDelayMin.Value;
                        int userMax = ModuleConfig.Instance.RandomWorkStationVirusDelayMax.Value;
                        if (userMax < userMin)
                        {
                            int origMin = userMin;
                            userMin = userMax;
                            userMax = origMin;
                        }

                        controller.virusInfectionTime = 0f;
                        controller.virusInfectionTimeLimit = Random.Range(userMin, userMax);

                        if (!controller.NetworkisFirewallActive
                            || controller.computerController.user.IsSlacker())
                        {
                            // Get Game Rule
                            float probability = GameManager.instance.virusProbability;

                            // RandomGen
                            if (Random.Range(0f, 100f) < probability * 100f)
                                controller.ServerSetVirus(true);
                        }
                    }
                }

                yield return null;
            }

            if (controller != null
                && !controller.WasCollected
                && _randomCoroutines.ContainsKey(controller))
                _randomCoroutines.Remove(controller);

            yield break;
        }
    }
}
