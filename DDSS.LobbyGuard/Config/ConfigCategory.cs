using MelonLoader;
using System.Collections.Generic;
using System.IO;

namespace DDSS_LobbyGuard.Config
{
    public class ConfigCategory
    {
        public static Dictionary<string, ConfigCategory> _allCategories = new();

        public MelonPreferences_Category Category;
        public string FileName = "Config.cfg";
        public eConfigType ConfigType;

        public ConfigCategory()
        {
            Init();

            string name = GetName();
            string displayName = GetDisplayName();

            Category = MelonPreferences.GetCategory(name);
            if (Category == null)
            {
                Category = MelonPreferences.CreateCategory(name, displayName, true, false);
                Category.DestroyFileWatcher();

                string filePath = Path.Combine(MelonMain._userDataPath, FileName);
                Category.SetFilePath(filePath, true, false);
            }

            CreatePreferences();
            Category.SaveToFile(false);

            if (!_allCategories.ContainsKey(name))
                _allCategories[name] = this;
        }
        public virtual void Init() { }

        public virtual string GetName() => null;
        public virtual string GetDisplayName() => GetName();
        public virtual void CreatePreferences() { }

        public void Save() => Category.SaveToFile(false);

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
