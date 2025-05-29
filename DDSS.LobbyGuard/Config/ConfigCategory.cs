using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;

namespace DDSS_LobbyGuard.Config
{
    public class ConfigCategory
    {
        private string _catID;

        public static SortedDictionary<string, ConfigCategory> _allCategories = new();

        public MelonPreferences_Category Category;
        public string FileName = "Config.cfg";

        public virtual eConfigType ConfigType { get => eConfigType.General; }
        public virtual string ID { get; }
        public virtual string DisplayName { get => ID; }

        public string CategoryID
        {
            get
            {
                if (_catID == null)
                {
                    string configType = Enum.GetName(typeof(eConfigType), ConfigType);
                    _catID = $"{configType}.{ID}";
                }
                return _catID;
            }
        }

        public ConfigCategory()
        {
            Category = MelonPreferences.GetCategory(CategoryID);
            if (Category == null)
            {
                Category = MelonPreferences.CreateCategory(CategoryID, DisplayName, true, false);
                Category.DestroyFileWatcher();

                string filePath = Path.Combine(MelonMain._userDataPath, FileName);
                Category.SetFilePath(filePath, true, false);
            }

            CreatePreferences();
            Category.SaveToFile(false);

            if (!_allCategories.ContainsKey(CategoryID))
                _allCategories[CategoryID] = this;
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
