using Il2Cpp;
using Il2CppLocalization;
using Il2CppSystem.Collections;
using Il2CppTMPro;
using Il2CppUMUI;
using Il2CppUMUI.UiElements;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    internal class MainMenuModSettings : ModSettingsBuilder
    {
        internal override void CreateTab()
        {
            // Find Settings Tab
            SettingsTab[] comps = Resources.FindObjectsOfTypeAll<SettingsTab>();
            foreach (SettingsTab comp in comps)
            {
                if (comp.gameObject.name != "SettingsTab")
                    continue;

                // Clone SettingsTab
                _tabObj = UnityEngine.Object.Instantiate(comp.gameObject, comp.transform.parent);
                _tabCasted = _tabObj.GetComponent<SettingsTab>();
                _tabObj.name = _tabCasted.name = "LobbyGuardSettings";

                // Get References
                _tab = _tabCasted;

                _categoryPrefab = _tabCasted.categoryPrefab;
                _categoryButtonPrefab = _tabCasted.categoryButtonPrefab;
                _categoryGrid = _tabCasted.categoryGrid;

                _settingPrefab = _tabCasted.settingPrefab;
                _settingParent = _tabCasted.settingsParent;
                _settingParentRect = _settingParent.gameObject.GetComponent<RectTransform>();

                // Fix Grid Layout
                GridLayoutGroup tabLayout = _settingParent.gameObject.GetComponent<GridLayoutGroup>();
                tabLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                tabLayout.constraintCount = 1;

                // Fix Content Layout
                ScaleContentToGridLayout scale = _settingParent.gameObject.AddComponent<ScaleContentToGridLayout>();
                scale.contentRectTransform = _settingParentRect;
                scale.gridLayoutGroup = tabLayout;

                // Initialize New Tab
                _tabCasted.InitTab();

                // Fix Scroll Rect
                ScrollRect scrollRect = _tabCasted.scrollRect;
                scrollRect.verticalNormalizedPosition = 1f;
                scrollRect.verticalScrollbar.value = 1f;
                scrollRect.SetDirty();

                // Fix Category Layout
                LayoutGroup canvasLayout = _categoryGrid.GetComponent<LayoutGroup>();
                canvasLayout.childAlignment = TextAnchor.MiddleCenter;

                // Change Clone Title
                Transform cloneTitleTrans = _tabObj.transform.Find("Tab/Tasks/TopBar/Title");
                if (cloneTitleTrans != null)
                {
                    TextMeshProUGUI cloneSettingsText = cloneTitleTrans.GetComponentInChildren<TextMeshProUGUI>();
                    if (cloneSettingsText != null)
                        cloneSettingsText.text = "LobbyGuard Settings";
                    LocalizedText localized = cloneTitleTrans.GetComponentInChildren<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Fix Close Button
                Transform closeButtonTrans = _tabObj.transform.Find("Canvas/LobbyGuardSettings/Tab/Tasks/TopBar/Close");
                if (closeButtonTrans != null)
                {
                    UMUIButton closeButton = closeButtonTrans.GetComponentInChildren<UMUIButton>();
                    if (closeButton != null)
                        closeButton.OnClick.AddListener((Action)ModSettingsFactory.OnClose);
                    LocalizedText localized = closeButton.GetComponentInChildren<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                break;
            }
        }

        internal override void CreatePanel()
        {
            // Get All Tabs
            UiTab[] tabs = Resources.FindObjectsOfTypeAll<UiTab>();
            if (tabs.Length <= 0)
                return;

            // Get About Panel
            GameObject aboutTab = null;
            foreach (UiTab tab in tabs)
                if (tab.gameObject.name == "About")
                {
                    aboutTab = tab.gameObject;
                    break;
                }
            if (aboutTab == null)
                return;

            // Clone the Panel
            _panelTabObj = UnityEngine.Object.Instantiate(aboutTab, aboutTab.transform.parent);
            _panelTabObj.name = Properties.BuildInfo.Name;
            _panelTabObj.transform.localPosition = new(0f, 430f, 0f);
            _panelTabObj.transform.SetSiblingIndex(aboutTab.transform.GetSiblingIndex() + 1);

            // Apply New Tab
            _panelTab = _panelTabObj.GetComponent<UiTab>();
            _panelTab.InitTab();
            UIManager.instance.tabs["LobbyGuardSettings"] = _panelTab;
            MenuTab[] menuTabs = Resources.FindObjectsOfTypeAll<MenuTab>();
            if (menuTabs.Length > 0)
                foreach (MenuTab tab in menuTabs)
                {
                    tab.childTabs.Add(_panelTab);
                    break;
                }

            // Get New Panel Title
            Transform titleTextTrans = _panelTabObj.transform.Find("NewsLetter/TopBar/Title");
            if (titleTextTrans != null)
            {
                // Set Text
                TextMeshProUGUI titleText = titleTextTrans.GetComponentInChildren<TextMeshProUGUI>();
                if (titleText != null)
                    titleText.text = $"{Properties.BuildInfo.Name} v{Properties.BuildInfo.Version}";

                // Remove Localization
                LocalizedText localized = titleTextTrans.GetComponentInChildren<LocalizedText>();
                if (localized != null)
                    UnityEngine.Object.Destroy(localized);
            }

            // Get New Panel Text
            Transform descriptionTextTrans = _panelTabObj.transform.Find("NewsLetter/Title");
            if (descriptionTextTrans == null)
                descriptionTextTrans = _panelTabObj.transform.Find("NewsLetter/Title (1)");
            if (descriptionTextTrans != null)
            {
                // Set Text
                TextMeshProUGUI descriptionText = descriptionTextTrans.GetComponentInChildren<TextMeshProUGUI>();
                if (descriptionText != null)
                    descriptionText.text = $"{Properties.BuildInfo.Description}\nCreated by {Properties.BuildInfo.Author}";

                // Remove Localization
                LocalizedText localized = descriptionTextTrans.GetComponentInChildren<LocalizedText>();
                if (localized != null)
                    UnityEngine.Object.Destroy(localized);
            }

            // Get New Panel Credits Button
            Transform creditsButtonTrans = _panelTabObj.transform.Find("NewsLetter/Credits");
            if (creditsButtonTrans != null)
            {
                // Setup GitHub Button
                ModSettingsFactory.SetupButton(
                    creditsButtonTrans,
                    new(-85, -66, 0),
                    "GitHub",
                    new Action(() => Application.OpenURL(Properties.BuildInfo.DownloadLink)));

                // Setup Settings Button
                Transform newButton = UnityEngine.Object.Instantiate(creditsButtonTrans, creditsButtonTrans.parent);
                ModSettingsFactory.SetupButton(
                    newButton,
                    new(85, -66, 0),
                    "Settings",
                    new Action(() => ModSettingsManager.OpenModSettings()));
            }
        }
    }
}
