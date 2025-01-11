using DDSS_LobbyGuard.Utils;
using Il2CppComputer.Scripts.System;
using Il2CppMirror;
using Il2CppPlayer.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace DDSS_LobbyGuard.Security
{
    internal static class ComputerSecurity
    {
        internal static Dictionary<string, Color> _playerAddresses = new();

        internal static bool ValidatePlayer(ComputerController computer,
            NetworkIdentity player)
        {
            // Validate ComputerController
            if ((computer == null)
                || (computer._workStationController == null)
                || (computer._workStationController.usingPlayer == null))
                return false;

            // Check if Actually Sitting in Seat
            if (computer._workStationController.usingPlayer != player)
                return false;

            // Validate player
            if (player.IsGhost())
                return false;

            // Player is Valid
            return true;
        }

        internal static bool EnforceClientEmailSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject)
                || string.IsNullOrWhiteSpace(subject)) 
                return false;

            if (subject.StartsWith("Re: "))
            {
                string[] split = subject.Split(' ', 2, System.StringSplitOptions.None);

                string documentName = split[1];
                if (string.IsNullOrEmpty(documentName)
                    || string.IsNullOrWhiteSpace(documentName))
                    return false;

                foreach (var doc in Task.documents)
                    if (doc.Item1 == documentName)
                        return true;

                return false;
            }

            return ((subject == "Nice")
                || (subject == "I'm down for it!"));
        }
    }
}
