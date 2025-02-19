using Il2Cpp;
using Il2CppLocalization;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DDSS_LobbyGuard.GUI.Internal
{
    internal static class ModSettingsPanelBuilder
    {
        internal static void Create(ref SettingsTab tab,
            ref RectTransform tabRect)
        {
            if (tab != null
               && !tab.WasCollected)
                return;

            // Find Settings Tab
            SettingsTab[] comps = Resources.FindObjectsOfTypeAll<SettingsTab>();
            foreach (SettingsTab comp in comps)
            {
                if (comp.gameObject.name != "SettingsTab")
                    continue;

                // Clone it for Mod Settings
                GameObject _settingsTabObj = Object.Instantiate(comp.gameObject, comp.transform.parent);
                tab = _settingsTabObj.GetComponent<SettingsTab>();
                tabRect = tab.settingsParent.GetComponent<RectTransform>();
                GridLayoutGroup tabLayout = tab.settingsParent.GetComponent<GridLayoutGroup>();
                tabLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                tabLayout.constraintCount = 1;
                ScaleContentToGridLayout scale = tab.settingsParent.gameObject.AddComponent<ScaleContentToGridLayout>();
                scale.contentRectTransform = tabRect;
                scale.gridLayoutGroup = tabLayout;
                _settingsTabObj.name = tab.name = "LobbyGuardSettings";
                tab.InitTab();
                tab.scrollRect.verticalNormalizedPosition = 1f;
                tab.scrollRect.verticalScrollbar.value = 1f;
                tab.scrollRect.SetDirty();

                // Change Clone Title
                Transform cloneTitleTrans = _settingsTabObj.transform.Find("Tab/Tasks/TopBar/Title");
                if (cloneTitleTrans != null)
                {
                    TextMeshProUGUI cloneSettingsText = cloneTitleTrans.GetComponentInChildren<TextMeshProUGUI>();
                    if (cloneSettingsText != null)
                        cloneSettingsText.text = "LobbyGuard Settings";
                    LocalizedText localized = cloneTitleTrans.GetComponentInChildren<LocalizedText>();
                    if (localized != null)
                        Object.Destroy(localized);
                }

                break;
            }
        }
    }
}
