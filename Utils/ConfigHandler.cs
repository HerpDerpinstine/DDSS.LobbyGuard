using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace DDSS_LobbyGuard.Utils
{
    internal static class ConfigHandler
    {
        private static MelonPreferences_Category _prefs_Category;
        internal static MelonPreferences_Entry<bool> _prefs_ExtendedInviteCodes;

        internal static void Setup()
        {
            // Create Preferences Category
            string FilePath = Path.Combine(MelonEnvironment.UserDataDirectory, "LobbyGuard.cfg");
            _prefs_Category = MelonPreferences.CreateCategory("LobbyGuard", "LobbyGuard");
            _prefs_Category.SetFilePath(FilePath, true);

            // Create Preferences Entries
            _prefs_ExtendedInviteCodes = CreatePref("ExtendedInviteCodes",
                "Extended Invite Codes",
                "Extends your Lobby Invite Codes to 8 Alpha-Numeric Characters instead of 4 Alpha Characters",
                false);

            // Save to File
            _prefs_Category.SaveToFile(false);
        }

        private static MelonPreferences_Entry<T> CreatePref<T>(
            string id,
            string displayName,
            string description,
            T defaultValue,
            bool isHidden = false)
            => _prefs_Category.CreateEntry(id,
                defaultValue,
                displayName,
                description,
                isHidden,
                false,
                null);
    }
}
