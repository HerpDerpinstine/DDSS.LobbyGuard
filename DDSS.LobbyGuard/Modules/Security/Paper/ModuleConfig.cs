using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Paper
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnPrintedDocuments;
        internal MelonPreferences_Entry<eConfigHostType> UsernamesOnPrintedImages;
        internal MelonPreferences_Entry<bool> PrinterCopiesSignatures;

        public ModuleConfig() : base()
            => Instance = this;
        public override eModuleType ConfigType
            => eModuleType.Security;
        public override string ID
            => "Paper";
        public override string DisplayName
            => "Paper";

        public override void CreatePreferences()
        {
            UsernamesOnPrintedDocuments = CreatePref("UsernamesOnPrintedDocuments",
                "Usernames On Printed Documents",
                "Puts the Player's Username in the Name of their Custom Printed Document",
                eConfigHostType.HOST_ONLY);

            UsernamesOnPrintedImages = CreatePref("UsernamesOnPrintedImages",
                "Usernames On Printed Images",
                "Puts the Player's Username in the Name of their Custom Printed Image",
                eConfigHostType.HOST_ONLY); 
            
            PrinterCopiesSignatures = CreatePref("PrinterCopiesSignatures",
                "Printer Copies Signatures",
                "Printer transfers the Signature of a Signed Document over to the Copy Document",
                false);
        }
    }
}
