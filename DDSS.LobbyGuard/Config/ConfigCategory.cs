using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDSS_LobbyGuard.Config
{
    public class ConfigCategory
    {
        public static SortedDictionary<string, ConfigCategory> _allCategories = new();

        public MelonPreferences_Category Category;
        public string FileExt = ".cfg";

        public virtual eConfigType ConfigType { get => eConfigType.General; }
        public virtual string ID { get; }
        public virtual string DisplayName { get => ID; }

        public ConfigCategory()
        {
            Category = MelonPreferences.GetCategory(ID);
            if (Category == null)
            {
                Category = MelonPreferences.CreateCategory(ID, DisplayName, true, false);
                Category.DestroyFileWatcher();

                string folderPath = Path.Combine(MelonMain._userDataPath, "Config");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string configType = Enum.GetName(typeof(eConfigType), ConfigType);
                string filePath = Path.Combine(folderPath, $"{configType}{FileExt}");
                Category.SetFilePath(filePath, true, false);
            }

            CreatePreferences();
            Category.SaveToFile(false);

            if (!_allCategories.ContainsKey(ID))
                _allCategories[ID] = this;
        }

        public virtual void CreatePreferences() { }

        public void Save() => Category.SaveToFile(false);

        public MelonPreferences_Entry<T> CreatePref<T>(
            string id,
            string displayName,
            string description,
            T defaultValue = default,
            bool isHidden = false)
        {
            var existingEntry = Category.GetEntry<T>(id);
            if (existingEntry != null)
                return existingEntry;

            return Category.CreateEntry(id,
                defaultValue,
                displayName,
                description,
                isHidden,
                false,
                null);
        }
    }
}
