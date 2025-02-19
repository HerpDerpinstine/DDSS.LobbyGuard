using MelonLoader;
using System.IO;

namespace DDSS_LobbyGuard.Config
{
    public class ConfigCategory
    {
        public MelonPreferences_Category Category;

        public ConfigCategory()
        {
            string filePath = Path.Combine(MelonMain._userDataPath, "Config.cfg");

            Category = MelonPreferences.GetCategory(GetName());
            if (Category == null)
            {
                Category = MelonPreferences.CreateCategory(GetName(), GetDisplayName(), true, false);
                Category.DestroyFileWatcher();
                Category.SetFilePath(filePath, true, false);
            }

            CreatePreferences();
            Category.SaveToFile(false);
        }

        public virtual string GetName() => null;
        public virtual string GetDisplayName() => GetName();
        public virtual void CreatePreferences() { }

        public void Save() => Category.SaveToFile();

        public MelonPreferences_Entry<T> CreatePref<T>(
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
