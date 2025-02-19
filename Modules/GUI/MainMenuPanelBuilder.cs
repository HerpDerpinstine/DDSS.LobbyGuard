using Il2Cpp;
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

        internal static void MainMenuInit()
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
            _newPanel.name = Properties.BuildInfo.Name;
            _newPanel.transform.localPosition = new(0f, 430f, 0f);
            _newPanel.transform.SetSiblingIndex(aboutTab.transform.GetSiblingIndex() + 1);

            // Apply New Tab
            UiTab newTab = _newPanel.GetComponent<UiTab>();
            newTab.InitTab();
            UIManager.instance.tabs["LobbyGuardSettings"] = newTab;
            MenuTab[] mainMenuTabs = Resources.FindObjectsOfTypeAll<MenuTab>();
            if (tabs.Length > 0)
                foreach (MenuTab tab in mainMenuTabs)
                {
                    tab.childTabs.Add(newTab);
                    break;
                }

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
                // Setup GitHub Button
                SetupButton(
                    creditsButtonTrans,
                    new(-85, -66, 0),
                    "GitHub",
                    new Action(() => Application.OpenURL(Properties.BuildInfo.DownloadLink)));

                // Setup Settings Button
                Transform newButton = GameObject.Instantiate(creditsButtonTrans, creditsButtonTrans.parent);
                SetupButton(
                    newButton,
                    new(85, -66, 0),
                    "Settings",
                    new Action(() => ModSettingsManager.OpenModSettings()));
            }

            ModSettingsManager.MainMenuInit();
        }

        internal static void SetupButton(Transform trans, Vector3 localPos, string text, Action onClick)
        {
            // Move Object
            trans.localPosition = localPos;

            // Rename Object
            trans.name = text;

            // Set Text
            TextMeshProUGUI creditsText = trans.GetComponentInChildren<TextMeshProUGUI>();
            if (creditsText != null)
                creditsText.text = text;

            // Remove Localization
            LocalizedText localized = trans.GetComponentInChildren<LocalizedText>();
            if (localized != null)
                UnityEngine.Object.Destroy(localized);

            // Apply new Button Callback
            UMUIButton cloneSettingsButton = trans.GetComponentInChildren<UMUIButton>();
            if (cloneSettingsButton != null)
            {
                // Create Event to Open Custom Tab
                cloneSettingsButton.OnClick = new();
                cloneSettingsButton.OnClick.AddListener(onClick);
            }
        }
    }
}
