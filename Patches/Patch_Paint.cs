using HarmonyLib;
using Il2Cpp;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DDSS_LobbyGuard.Patches
{
    [HarmonyPatch]
    internal class Patch_Paint
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Paint), nameof(Paint.Start))]
        private static bool Start_Prefix(Paint __instance)
        {
            foreach (Color color in __instance.colors)
            {
                GameObject gameObject = GameObject.Instantiate(__instance.colorBtnPrefab, __instance.colorBtnParent);
                gameObject.GetComponentsInChildren<Image>()[1].color = color;
                gameObject.GetComponent<CursorButton>().OnClick.AddListener(new Action(() => __instance.SetColor(__instance.colors.IndexOf(color))));
            }

            __instance.SetColor(0);
            __instance.censorDrawings = GameSettingsManager.instance.GetSetting("Censor Drawings") == 1f;

            // Prevent Original
            return false;
        }
    }
}
