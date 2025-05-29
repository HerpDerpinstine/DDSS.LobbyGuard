using Il2Cpp;
using Il2CppTMPro;
using UnityEngine;

namespace DDSS_LobbyGuard.Modules.General.GUI.Internal
{
    internal static class ModSettingsCategoryBuilder
    {
        internal static void Create(string name)
        {
            GameObject catObj = Object.Instantiate(ModSettingsManager._tab.categoryPrefab,
                ModSettingsManager._tab.settingsParent);
            TextMeshProUGUI catTmp = catObj.GetComponentInChildren<TextMeshProUGUI>();
            catTmp.text = name;
            catTmp.autoSizeTextContainer = true;
        }

        internal static CategoryButton CreateButton(string name)
        {
            GameObject catObj = Object.Instantiate(ModSettingsManager._tab.categoryButtonPrefab,
                ModSettingsManager._tab.categoryGrid);
            TextMeshProUGUI catTmp = catObj.GetComponentInChildren<TextMeshProUGUI>();
            catTmp.text = name;
            return catObj.GetComponent<CategoryButton>();
        }
    }
}
