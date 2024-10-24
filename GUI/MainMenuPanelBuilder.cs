﻿using Il2Cpp;
using Il2CppLocalization;
using Il2CppTMPro;
using Il2CppUMUI;
using Il2CppUMUI.UiElements;
using System;
using UnityEngine;

namespace DDSS_LobbyGuard.GUI
{
    internal static class MainMenuPanelBuilder
    {
        private static GameObject _newPanel;

        internal static void OnSceneLoad()
        {
            if ((_newPanel != null)
                && !_newPanel.WasCollected)
                return;

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
            _newPanel = GameObject.Instantiate(aboutTab, aboutTab.transform.parent);
            _newPanel.transform.localPosition = new(0.2f, 430.9f, 0f);
            _newPanel.name = Properties.BuildInfo.Name;

            // Apply New Tab
            UiTab newTab = _newPanel.GetComponent<UiTab>();
            newTab.InitTab();
            UIManager.instance.tabs[Properties.BuildInfo.Name] = newTab;
            MenuTab[] mainMenuTabs = Resources.FindObjectsOfTypeAll<MenuTab>();
            if (tabs.Length > 0)
                foreach (MenuTab tab in mainMenuTabs)
                {
                    tab.childTabs.Add(newTab);
                    break;
                }

            // Fix Panel Layering
            int siblingIndex = aboutTab.transform.GetSiblingIndex() - 1;
            if (siblingIndex < 0)
                siblingIndex = 0;
            _newPanel.transform.SetSiblingIndex(siblingIndex);

            // Get New Panel Title
            Transform titleTextTrans = _newPanel.transform.Find("NewsLetter/TopBar/Title");
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
            Transform descriptionTextTrans = _newPanel.transform.Find("NewsLetter/Title");
            if (descriptionTextTrans == null)
                descriptionTextTrans = _newPanel.transform.Find("NewsLetter/Title (1)");
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
            Transform creditsButtonTrans = _newPanel.transform.Find("NewsLetter/Credits");
            if (creditsButtonTrans != null)
            {
                // Rename Object
                creditsButtonTrans.name = "GitHub";
                
                // Set Text
                TextMeshProUGUI creditsText = creditsButtonTrans.GetComponentInChildren<TextMeshProUGUI>();
                if (creditsText != null)
                    creditsText.text = "GitHub";

                // Remove Localization
                LocalizedText localized = creditsButtonTrans.GetComponentInChildren<LocalizedText>();
                if (localized != null)
                    UnityEngine.Object.Destroy(localized);

                // Apply new Button Callback
                UMUIButton cloneSettingsButton = creditsButtonTrans.GetComponentInChildren<UMUIButton>();
                if (cloneSettingsButton != null)
                {
                    // Create Event to Open Custom Tab
                    cloneSettingsButton.OnClick = new();
                    cloneSettingsButton.OnClick.AddListener(new Action(() => Application.OpenURL(Properties.BuildInfo.DownloadLink)));
                }
            }
        }
    }
}
