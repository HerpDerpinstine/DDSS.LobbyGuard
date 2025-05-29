using System;

namespace DDSS_LobbyGuard.Modules.Extras.ExtendedInviteCodes
{
    internal class ModuleMain : ILobbyModule
    {
        // Old Codes: 14950 possible combinations
        // New Codes: 30260340 possible combinations
        private const string AllowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const int CharacterCount = 8;

        public override string Name => "ExtendedInviteCodes";
        public override eModuleType ModuleType => eModuleType.Extras;
        public override Type ConfigType => typeof(ModuleConfig);

        internal static string GenerateNewCode()
        {
            string newCode = "";
            for (int i = 0; i < CharacterCount; i++)
                newCode += AllowedCharacters[UnityEngine.Random.Range(0, AllowedCharacters.Length - 1)];
            return newCode;
        }
    }
}