using Il2CppMirror;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Utils
{
    public static class RPCHelper
    {
        public enum eType
        {
            NONE,
            LobbyPlayer_RpcSetPlayerRole,
            ComputerController_RpcClick,
            ComputerController_RpcCursorUp,
        }

        private static Dictionary<eType, (string, int)> _typeToQuery = new()
        {
            [eType.LobbyPlayer_RpcSetPlayerRole] = ("Player.Lobby.LobbyPlayer::RpcSetPlayerRole", 0),
            [eType.ComputerController_RpcClick] = ("Computer.Scripts.System.ComputerController::RpcClick", 0),
            [eType.ComputerController_RpcCursorUp] = ("Computer.Scripts.System.ComputerController::RpcCursorUp", 0),
        };

        public static (string, int) Get(eType type)
            => _typeToQuery[type];

        public static void OnRegister(string functionFullName)
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
