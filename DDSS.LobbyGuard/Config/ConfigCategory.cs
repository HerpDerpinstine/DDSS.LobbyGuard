﻿using DDSS_LobbyGuard.Modules;
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

        public virtual eModuleType ConfigType { get; set; }
        public virtual string ID { get; set; }
        public virtual string DisplayName { get; set; }

        public ConfigCategory()
        {
            string folderPath = Path.Combine(MelonMain._userDataPath, "Config");
            string fileName = Enum.GetName(typeof(eModuleType), ConfigType);

            Setup(folderPath, fileName, ID, DisplayName, ConfigType);

            if (!_allCategories.ContainsKey(ID))
                _allCategories[ID] = this;
        }

        public ConfigCategory(string folderPath,
            string fileName,
            string categoryID,
            string categoryDisplayName,
            eModuleType categoryType)
            => Setup(folderPath, fileName, categoryID, categoryDisplayName, categoryType);

        public void Setup(string folderPath,
            string fileName,
            string categoryID,
            string categoryDisplayName,
            eModuleType categoryType)
        {
            if (string.IsNullOrEmpty(categoryDisplayName)
                || string.IsNullOrWhiteSpace(categoryDisplayName))
                categoryDisplayName = categoryID;

            ID = categoryID;
            DisplayName = categoryDisplayName;
            ConfigType = categoryType;

            Category = MelonPreferences.GetCategory(ID);
            if (Category == null)
            {
                Category = MelonPreferences.CreateCategory(ID, DisplayName, true, false);
                Category.DestroyFileWatcher();

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filePath = Path.Combine(folderPath, $"{fileName}{FileExt}");
                Category.SetFilePath(filePath, true, false);
            }

            CreatePreferences();
            Category.SaveToFile(false);
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
