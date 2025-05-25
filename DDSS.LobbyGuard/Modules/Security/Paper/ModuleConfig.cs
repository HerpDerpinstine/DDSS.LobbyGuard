using DDSS_LobbyGuard.Config;
using MelonLoader;

namespace DDSS_LobbyGuard.Modules.Security.Paper
{
    internal class ModuleConfig : ConfigCategory
    {
        internal static ModuleConfig Instance { get; private set; }

        internal MelonPreferences_Entry<bool> UsernamesOnPrintedDocuments;
        internal MelonPreferences_Entry<bool> UsernamesOnPrintedImages;
        internal MelonPreferences_Entry<bool> PrinterCopiesSignatures;

        public ModuleConfig() : base()
            => Instance = this;
        public override void Init()
            => ConfigType = eConfigType.Security;
        public override string GetName()
            => "Paper";
        public override string GetDisplayName()
            => "Paper";

        public override void CreatePreferences()
        {
            UsernamesOnPrintedDocuments = CreatePref("UsernamesOnPrintedDocuments",
                "Usernames On Printed Documents",
                "Puts the Player's Username in the Name of their Custom Printed Document",
                true);

            UsernamesOnPrintedImages = CreatePref("UsernamesOnPrintedImages",
                "Usernames On Printed Images",
                "Puts the Player's Username in the Name of their Custom Printed Image",
                true); 
            
            PrinterCopiesSignatures = CreatePref("PrinterCopiesSignatures",
                "Printer Copies Signatures",
                "Printer transfers the Signature of a Signed Document over to the Copy Document",
                false);
        }
    }
}
