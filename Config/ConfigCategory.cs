using DDSS_LobbyGuard.GUI.Internal;
using MelonLoader;
using System.IO;

namespace DDSS_LobbyGuard.Config
{
    internal class ConfigCategory
    {
        internal MelonPreferences_Category Category;

        internal ConfigCategory()
        {
            string filePath = Path.Combine(MelonMain._userDataPath, "Config.cfg");
            Category = MelonPreferences.CreateCategory(GetName(), GetDisplayName());
            Category.IsHidden = true;
            Category.SetFilePath(filePath, true, false);
            CreatePreferences();
            Category.SaveToFile(false);
            ModSettingsFactory._validCategories.Add(Category);
        }

        internal virtual string GetName() => null;
        internal virtual string GetDisplayName() => GetName();
        internal virtual void CreatePreferences() { }

        internal void Save() => Category.SaveToFile();

        internal MelonPreferences_Entry<T> CreatePref<T>(
            string id,
            string displayName,
            string description,
            T defaultValue = default,
            bool isHidden = false)
            => Category.CreateEntry(id,
                defaultValue,
                displayName,
                description,
                isHidden,
                false,
                null);
    }
}
