using Il2CppMirror;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Utils
{
    internal static class RPCHelper
    {
        internal enum eType
        {
            NONE,
            LobbyPlayer_RpcSetPlayerRole,
            ComputerController_RpcClick,
            ComputerController_RpcCursorUp,
            ComputerController_RpcSyncCursor,
        }

        private static Dictionary<eType, (string, int)> _typeToQuery = new()
        {
            [eType.LobbyPlayer_RpcSetPlayerRole] = ("Player.Lobby.LobbyPlayer::RpcSetPlayerRole", 0),
            [eType.ComputerController_RpcClick] = ("Il2CppComputer.Scripts.System.ComputerController::RpcClick", 0),
            [eType.ComputerController_RpcCursorUp] = ("Il2CppComputer.Scripts.System.ComputerController::RpcCursorUp", 0),
            [eType.ComputerController_RpcSyncCursor] = ("Il2CppComputer.Scripts.System.ComputerController::RpcSyncCursor", 0),
        };

        internal static (string, int) Get(eType type)
            => _typeToQuery[type];

        internal static void OnRegister(string functionFullName)
        {
            foreach (var pair in _typeToQuery)
            {
                if (!functionFullName.Contains(pair.Value.Item1))
                    continue;

                (string, int) info = _typeToQuery[pair.Key];
                info.Item1 = functionFullName;
                info.Item2 = (ushort)(functionFullName.GetStableHashCode() & 65535);
                _typeToQuery[pair.Key] = info;
            }
        }
    }
}
