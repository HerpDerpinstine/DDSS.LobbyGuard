using Il2CppComputer.Scripts.System;
using Il2CppMirror;

namespace DDSS_LobbyGuard.Security
{
    internal static class ComputerSecurity
    {
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

            // Validate Distance
            if (!InteractionSecurity.IsWithinRange(player.transform.position, computer.transform.position))
                return false;

            // Player is Valid
            return true;
        }
    }
}
