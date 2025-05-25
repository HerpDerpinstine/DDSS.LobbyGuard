using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Extras.MoreRoleSettings
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> AllowSpecialistsToPourInk;

        internal MelonPreferences_Entry<int> SlackerTrashBinFireDelay;

        internal MelonPreferences_Entry<bool> AllowJanitorsToPourInk;
        internal MelonPreferences_Entry<bool> AllowJanitorsToLockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUnlockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUpdateCCTV;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUpdateComputers;
        internal MelonPreferences_Entry<bool> AllowJanitorsToKeepWorkStation;

        public ModuleConfig() : base()
        {
            if (Instance == null)
                Instance = this;
        }

        public override void Init()
            => ConfigType = eConfigType.Extras;
        public override string GetName()
            => "MoreRoleSettings";
        public override string GetDisplayName()
            => "More Role Settings";

        public override void CreatePreferences()
        {
            AllowSpecialistsToPourInk = CreatePref("AllowSpecialistsToPourInk",
                "Allow Specialists to Pour Ink",
                "Allows Specialists to Pour Ink in the Fish Tank",
                true);

            SlackerTrashBinFireDelay = CreatePref("SlackerTrashBinFireDelay",
                 "Slacker TrashBin Fire Delay",
                 "Seconds until TrashBin Fire Ignites from Slacker Task",
                 4);

            AllowJanitorsToPourInk = CreatePref("AllowJanitorsToPourInk",
                "Allow Janitors to Pour Ink",
                "Allows Janitors to Pour Ink in the Fish Tank",
                true);

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

            AllowJanitorsToUpdateComputers = CreatePref("AllowJanitorsToUpdateComputers",
                "Allow Janitors to Update Computers",
                "Allows Janitors to do Software Updates on Computers",
                true);

            AllowJanitorsToKeepWorkStation = CreatePref("AllowJanitorsToKeepWorkStation",
                "Allow Janitors to Keep WorkStation",
                "Allows Janitors to Keep their Assigned WorkStation",
                false);
        }
    }
}
