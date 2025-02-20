using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreJanitorSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> AllowJanitorsToLockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUnlockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUpdateCCTV;
        //internal MelonPreferences_Entry<bool> AllowJanitorsToKeepWorkStation;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreJanitorSettings";
        public override string GetDisplayName()
            => "More Janitor Settings";

        public override void CreatePreferences()
        {
            AllowJanitorsToLockDoors = CreatePref("AllowJanitorsToLockDoors",
                "Allow Janitors to Lock Doors",
                "Allows Janitors to Lock Doors",
                true);

            AllowJanitorsToUnlockDoors = CreatePref("AllowJanitorsToUnlockDoors",
                "Allow Janitors to Unlock Doors",
                "Allows Janitors to Unlock Doors",
                true);

            AllowJanitorsToUpdateCCTV = CreatePref("AllowJanitorsToUpdateCCTV",
                "Allow Janitors to Update CCTV",
                "Allows Janitors to Update Firmware on CCTV Cameras",
                true);

            //AllowJanitorsToKeepWorkStation = CreatePref("AllowJanitorsToKeepWorkStation",
            //    "Allow Janitors to Keep WorkStation",
            //    "Allows Janitors to Keep their Assigned WorkStation",
            //    false);
        }
    }
}
