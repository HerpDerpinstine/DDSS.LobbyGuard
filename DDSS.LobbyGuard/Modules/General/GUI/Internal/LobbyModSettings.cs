using Il2Cpp;
using Il2CppLocalization;
using Il2CppTMPro;
using Il2CppUMUI;
using Il2CppUMUI.UiElements;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    internal class LobbyModSettings : ModSettingsBuilder
    {
        internal override void CreateTab()
        {
            // Find Settings Tab
            LobbySettingsTab[] comps = Resources.FindObjectsOfTypeAll<LobbySettingsTab>();
            foreach (LobbySettingsTab comp in comps)
            {
                if (comp.gameObject.name != "LobbySettingsTab")
                    continue;

                // Clone SettingsTab
                _tabObj = UnityEngine.Object.Instantiate(_tabStatic, comp.transform.parent);

                _tabCasted = _tabObj.GetComponent<SettingsTab>();
                _tabCasted.isOverlay = true;

                _tabObj.name = _tabCasted.name = ModSettingsManager._tabID;

                // Get References
                _tab = _tabCasted;

                _categoryPrefab = _tabCasted.categoryPrefab;
                _categoryButtonPrefab = _tabCasted.categoryButtonPrefab;
                _categoryGrid = _tabCasted.categoryGrid;

                _settingPrefab = _tabCasted.settingPrefab;
                _settingParent = _tabCasted.settingsParent;
                _settingParentRect = _settingParent.GetComponent<RectTransform>();

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
                Transform cloneTitleTrans = _tabObj.transform.FindChild("Tab/Tasks/TopBar/Title");
                if (cloneTitleTrans != null)
                {
                    Vector3 titlePos = cloneTitleTrans.localPosition;
                    titlePos.x = 0f;
                    cloneTitleTrans.localPosition = titlePos;

                    TextMeshProUGUI cloneSettingsText = cloneTitleTrans.GetComponent<TextMeshProUGUI>();
                    if (cloneSettingsText != null)
                    {
                        cloneSettingsText.text = " LobbyGuard Settings";
                        cloneSettingsText.alignment = TextAlignmentOptions.Center;
                    }

                    LocalizedText localized = cloneTitleTrans.GetComponent<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Fix Close Button
                Transform closeButtonTrans = _tabObj.transform.FindChild("Tab/Tasks/TopBar/Close");
                if (closeButtonTrans != null)
                {
                    UMUIButton closeButton = closeButtonTrans.GetComponent<UMUIButton>();
                    if (closeButton != null)
                        closeButton.OnClick.AddListener(new Action(ModSettingsFactory.OnClose));
                    LocalizedText localized = closeButton.GetComponent<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                break;
            }
        }

        internal override void CreatePanel()
        {
            // Find Settings Tab
            GameRulesSettingsTab[] comps = Resources.FindObjectsOfTypeAll<GameRulesSettingsTab>();
            foreach (GameRulesSettingsTab comp in comps)
            {
                // Clone the Panel
                _panelTabObj = UnityEngine.Object.Instantiate(_panelTabStatic, comp.transform.parent);
                _panelTabObj.name = Properties.BuildInfo.Name;
                _panelTabObj.transform.localPosition = new(0f, 430f, 0f);
                _panelTabObj.transform.SetSiblingIndex(comp.transform.GetSiblingIndex() + 1);
                _panelTabObj.SetActive(true);

                // Apply New Tab
                _panelTab = _panelTabObj.GetComponent<UiTab>();
                _panelTab.InitTab();
                UIManager.instance.tabs[ModSettingsManager._tabID] = _panelTab;
                MenuTab[] menuTabs = Resources.FindObjectsOfTypeAll<MenuTab>();
                if (menuTabs.Length > 0)
                    foreach (MenuTab tab in menuTabs)
                    {
                        tab.childTabs.Add(_panelTab);
                        break;
                    }
                
                // Get New Panel Title
                Transform titleTextTrans = _panelTabObj.transform.FindChild("NewsLetter/TopBar/Title");
                if (titleTextTrans != null)
                {
                    Vector3 titlePos = titleTextTrans.localPosition;
                    titlePos.x = 0f;
                    titleTextTrans.localPosition = titlePos;

                    // Set Text
                    TextMeshProUGUI titleText = titleTextTrans.GetComponent<TextMeshProUGUI>();
                    if (titleText != null)
                    {
                        titleText.text = $"{Properties.BuildInfo.Name} v{Properties.BuildInfo.Version}";
                        titleText.alignment = TextAlignmentOptions.Center;
                    }

                    // Remove Localization
                    LocalizedText localized = titleTextTrans.GetComponent<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Get New Panel Text
                Transform descriptionTextTrans = _panelTabObj.transform.FindChild("NewsLetter/Title");
                if (descriptionTextTrans == null)
                    descriptionTextTrans = _panelTabObj.transform.FindChild("NewsLetter/Title (1)");
                if (descriptionTextTrans != null)
                {
                    // Set Text
                    TextMeshProUGUI descriptionText = descriptionTextTrans.GetComponent<TextMeshProUGUI>();
                    if (descriptionText != null)
                    {
                        descriptionText.text = $"{Properties.BuildInfo.Description}\nCreated by {Properties.BuildInfo.Author}";
                        descriptionText.alignment = TextAlignmentOptions.Center;
                    }

                    // Remove Localization
                    LocalizedText localized = descriptionTextTrans.GetComponent<LocalizedText>();
                    if (localized != null)
                        UnityEngine.Object.Destroy(localized);
                }

                // Fix Lobby Settings
                Transform lobbyButtonTrans = comp.transform.FindChild("Tab/Settings/Ready");
                if (lobbyButtonTrans != null)
                {
                    UMUIButton lobbyButton = lobbyButtonTrans.GetComponent<UMUIButton>();
                    if (lobbyButton != null)
                    {
                        var originalClick = lobbyButton.OnClick;
                        lobbyButton.OnClick = new();
                        lobbyButton.OnClick.AddListener(new Action(() =>
                        {
                            UIManager.instance.CloseTab(1);
                            if (originalClick != null)
                                originalClick.Invoke();
                            ModSettingsFactory.OnClose();
                        }));
                    }
                }

                // Setup GitHub Button
                Transform githubButtonTrans = _panelTabObj.transform.FindChild("NewsLetter/GitHub");
                if (githubButtonTrans != null)
                    ModSettingsFactory.SetupButton(
                        githubButtonTrans,
                        new(-85, -66, 0),
                        "GitHub",
                        new Action(() => Application.OpenURL(Properties.BuildInfo.DownloadLink)));

                // Setup Settings Button
                Transform newButton = _panelTabObj.transform.FindChild("NewsLetter/Settings");
                if (newButton != null)
                    ModSettingsFactory.SetupButton(
                        newButton,
                        new(85, -66, 0),
                        "Settings",
                        new Action(() =>
                        {
                            UIManager.instance.CloseTab(1);
                            ModSettingsManager.OpenModSettings();
                        }));
                
                break;
            }
        }
    }
}
