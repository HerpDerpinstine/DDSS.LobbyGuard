﻿namespace DDSS_LobbyGuard.Security
{
    internal static class InviteCodeSecurity
    {
        const string AllowedCharacters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const int CharacterCount = 8;

        internal static string GenerateNewCode()
        {
            string newCode = "";
            for (int i = 0; i < CharacterCount; i++)
                newCode += AllowedCharacters[UnityEngine.Random.Range(0, AllowedCharacters.Length)];
            return newCode;
        }
    }
}