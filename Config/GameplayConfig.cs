using MelonLoader;

namespace DDSS_LobbyGuard.Config
{
    internal class GameplayConfig : ConfigCategory
    {
        internal MelonPreferences_Entry<bool> UsernamesOnPrintedDocuments;
        internal MelonPreferences_Entry<bool> UsernamesOnPrintedImages;
        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnStickyNotes;

        internal MelonPreferences_Entry<int> MaxCharactersOnDocuments;
        internal MelonPreferences_Entry<int> MaxCharactersOnStickyNotes;
        internal MelonPreferences_Entry<int> MaxCharactersOnChatMessages;

        internal MelonPreferences_Entry<bool> StickyNotesOnPlayers;
        internal MelonPreferences_Entry<bool> StickyNotesOnDoors;

        internal MelonPreferences_Entry<bool> PrinterCopiesWithSignature;

        internal MelonPreferences_Entry<bool> CloseDoorsOnLock;

        internal MelonPreferences_Entry<bool> SpawnManagerKeys;
        internal MelonPreferences_Entry<bool> SpawnStereoCDs;
        internal MelonPreferences_Entry<bool> SpawnUnassignedDeskItems;

        internal MelonPreferences_Entry<bool> AllowJanitorsToVote;
        internal MelonPreferences_Entry<bool> AllowJanitorsToPourInk;
        internal MelonPreferences_Entry<bool> AllowJanitorsToLockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUnlockDoors;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUpdateCCTV;
        internal MelonPreferences_Entry<bool> AllowJanitorsToUpdateComputers;
        internal MelonPreferences_Entry<bool> AllowJanitorsToKeepWorkStation;

        internal MelonPreferences_Entry<bool> AllowSpecialistsToPourInk;

        internal MelonPreferences_Entry<bool> AllowVirusesWhileServerIsDown;

        internal MelonPreferences_Entry<bool> ProductivityFromTaskCompletion;

        internal MelonPreferences_Entry<bool> PlayerLeavesReduceTerminations;

        internal MelonPreferences_Entry<int> SlackerTrashBinFireDelay;
        internal MelonPreferences_Entry<int> SlackerServerOutageDelay;

        internal MelonPreferences_Entry<bool> ServerOutageResetsRandomOutageTimer;
        internal MelonPreferences_Entry<int> RandomServerOutageDelayMin;
        internal MelonPreferences_Entry<int> RandomServerOutageDelayMax;

        internal MelonPreferences_Entry<bool> WorkStationVirusTurnsOffFirewall;
        internal MelonPreferences_Entry<bool> WorkStationVirusResetsRandomVirusTimer;
        internal MelonPreferences_Entry<int> RandomWorkStationVirusDelayMin;
        internal MelonPreferences_Entry<int> RandomWorkStationVirusDelayMax;

        internal MelonPreferences_Entry<bool> EnforceComputerWindowBoundary;

        //internal MelonPreferences_Entry<bool> PlayerVelocityEnforcement;
        internal MelonPreferences_Entry<bool> HideSlackersFromClients;

        internal override string GetName()
            => "Gameplay";

        internal override void CreatePreferences()
        {
            UsernamesOnPrintedDocuments = CreatePref("UsernamesOnPrintedDocuments",
                "Usernames On Printed Documents",
                "Puts the Player's Username in the Name of their Custom Printed Document",
                true);

            UsernamesOnPrintedImages = CreatePref("UsernamesOnPrintedImages",
                "Usernames On Printed Images",
                "Puts the Player's Username in the Name of their Custom Printed Image",
                true);

            UsernamesOnStickyNotes = CreatePref("UsernamesOnStickyNotes",
                "Usernames On Sticky Notes",
                "Puts the Player's Username in the Name of their Custom Sticky Note   [ ALL | HOST_ONLY | DISABLED ]",
                eConfigHostType.HOST_ONLY);

            MaxCharactersOnDocuments = CreatePref("MaxCharactersOnDocuments",
                "Max Characters on Documents",
                "Max Characters allowed on Documents",
                240);

            MaxCharactersOnStickyNotes = CreatePref("MaxCharactersOnStickyNotes",
                "Max Characters on Sticky Notes",
                "Max Characters allowed on Sticky Notes",
                100);

            MaxCharactersOnChatMessages = CreatePref("MaxCharactersOnChatMessages",
                "Max Characters on Chat Messages",
                "Max Characters allowed on Chat Messages",
                180);

            StickyNotesOnPlayers = CreatePref("StickyNotesOnPlayers",
                "Sticky Notes on Players",
                "Allows the Grabbing and Placing of Sticky Notes on Players",
                true);

            StickyNotesOnDoors = CreatePref("StickyNotesOnDoors",
                "Sticky Notes on Doors",
                "Allows the Grabbing and Placing of Sticky Notes on Doors",
                true);

            PrinterCopiesWithSignature = CreatePref("PrinterCopiesWithSignature",
                "Printer Copies with Signature",
                "Printer transfers the Signature of a Signed Document over to the Copy Document",
                false);

            CloseDoorsOnLock = CreatePref("CloseDoorsOnLock",
                "Close Doors On Lock",
                "Close Doors when Locking",
                true);

            SpawnManagerKeys = CreatePref("SpawnManagerKeys",
                "Spawn Manager Keys",
                "Allows Spawning of Keys in Manager's Office",
                true);

            SpawnStereoCDs = CreatePref("SpawnStereoCDs",
                "Spawn Stereo CDs",
                "Allows Spawning of CDs next to the Stereo",
                true);

            SpawnUnassignedDeskItems = CreatePref("SpawnUnassignedDeskItems",
                "Spawn Unassigned Desk Items",
                "Spawns a Mug and Mouse on Desks that haven't been Assigned",
                false);

            AllowJanitorsToVote = CreatePref("AllowJanitorsToVote",
                "Allow Janitors to Vote",
                "Allows Janitors to Vote on Manager",
                false);

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

            AllowSpecialistsToPourInk = CreatePref("AllowSpecialistsToPourInk",
                "Allow Specialists to Pour Ink",
                "Allows Specialists to Pour Ink in the Fish Tank",
                true);

            AllowVirusesWhileServerIsDown = CreatePref("AllowVirusesWhileServerIsDown",
                "Allow Viruses while Server is Down",
                "Allows WorkStations to get Viruses while the Server is Down",
                true);

            ProductivityFromTaskCompletion = CreatePref("ProductivityFromTaskCompletion",
                "Productivity from Task Completion",
                "Allows Productivity to be Gained from Task Completion",
                true);

            PlayerLeavesReduceTerminations = CreatePref("PlayerLeavesReduceTerminations",
                "Player Leaves Reduce Terminations",
                "Allows Terminations to be Decreased from Players Disconnecting",
                true);

            SlackerTrashBinFireDelay = CreatePref("SlackerTrashBinFireDelay",
                "Slacker TrashBin Fire Delay",
                "Seconds until TrashBin Fire Ignites from Slacker Task",
                4);

            SlackerServerOutageDelay = CreatePref("SlackerServerOutageDelay",
                "Slacker Server Outage Delay",
                "Seconds until Server Outage from Slacker Task",
                6);

            ServerOutageResetsRandomOutageTimer = CreatePref("ServerOutageResetsRandomOutageTimer",
                "Server Outage resets Random Outage Timer",
                "Server Outages will Reset the Random Outage Timer",
                false);

            RandomServerOutageDelayMin = CreatePref("RandomServerOutageDelayMin",
                "Random Server Outage Delay Min",
                "Min Delay in Seconds until the Server has a Random Outage",
                120);

            RandomServerOutageDelayMax = CreatePref("RandomServerOutageDelayMax",
                "Random Server Outage Delay Max",
                "Max Delay in Seconds until the Server has a Random Outage",
                600);

            WorkStationVirusTurnsOffFirewall = CreatePref("WorkStationVirusTurnsOffFirewall",
                "WorkStation Virus turns off Firewall",
                "WorkStation Viruses will Automatically Turn Off the Firewall",
                false);

            WorkStationVirusResetsRandomVirusTimer = CreatePref("WorkStationVirusResetsRandomVirusTimer",
                "WorkStation Virus resets Random Virus Timer",
                "WorkStation Viruses will Reset the Random Virus Timer",
                true);

            RandomWorkStationVirusDelayMin = CreatePref("RandomWorkStationVirusDelayMin",
                "Random WorkStation Virus Delay Min",
                "Min Delay in Seconds until a WorkStation rolls the Dice on getting a Random Virus",
                30);

            RandomWorkStationVirusDelayMax = CreatePref("RandomWorkStationVirusDelayMax",
                "Random WorkStation Virus Delay Max",
                "Max Delay in Seconds until a WorkStation rolls the Dice on getting a Random Virus",
                60);

            EnforceComputerWindowBoundary = CreatePref("EnforceComputerWindowBoundary",
                "Enforce Computer Window Boundary",
                "Prevents Dragging Windows on WorkStation Computers outside of the Monitor's Boundaries",
                false);

            //PlayerVelocityEnforcement = CreatePref("EnforcePlayerVelocity",
            //    "Enforce Player Velocity",
            //    "Prevents Speedhacking using Context-Based Velocity Clamping",
            //    false);

            HideSlackersFromClients = CreatePref("HideSlackersFromClients",
                "Hide Slackers from Clients",
                "Prevents Slackers and Slacker Count from being broadcasted to Clients",
                false);
        }
    }
}
