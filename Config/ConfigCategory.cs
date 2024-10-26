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
            Category = CreateCategory(filePath);
            CreatePreferences();
            Category.SaveToFile(false);
        }

        internal virtual MelonPreferences_Category CreateCategory(string filePath) => default;
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
