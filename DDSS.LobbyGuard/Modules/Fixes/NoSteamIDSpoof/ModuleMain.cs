using Il2CppSteamworks;
using System;
using System.Collections.Generic;

namespace DDSS_LobbyGuard.Modules.Fixes.NoSteamIDSpoof
{
    internal class ModuleMain : ILobbyModule
    {
        private static List<ulong> _validIds = new List<ulong>();
        private Callback<LobbyChatUpdate_t> lobbyChatUpdate;
        private Callback<LobbyChatUpdate_t> lobbyChatUpdateServer;

        internal static bool IsSteamLobby = false;

        public override string Name => "Fixes.NoSteamIDSpoof";
        public override int Priority => -100;

        public override bool OnLoad()
        {
            lobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(new Action<LobbyChatUpdate_t>(OnLobbyChatUpdate));
            lobbyChatUpdateServer = Callback<LobbyChatUpdate_t>.CreateGameServer(new Action<LobbyChatUpdate_t>(OnLobbyChatUpdate));
            return true;
        }

        public override void OnQuit()
        {
            _validIds.Clear();
            if (lobbyChatUpdate != null)
            {
                lobbyChatUpdate.Dispose();
                lobbyChatUpdate = null;
            }
            if (lobbyChatUpdateServer != null)
            {
                lobbyChatUpdateServer.Dispose();
                lobbyChatUpdateServer = null;
            }
        }

        public override void OnSceneInit(int buildIndex, string sceneName)
        {
            if (sceneName != "MainMenuScene")
                return;
            _validIds.Clear();
        }

        internal static bool IsSteamIDValid(ulong steamID)
            => _validIds.Contains(steamID);

        internal static void AddValidSteamID(ulong steamID)
        {
            if (IsSteamIDValid(steamID))
                return;
            _validIds.Add(steamID);
        }

        internal static void RemoveValidSteamID(ulong steamID)
        {
            if (!IsSteamIDValid(steamID))
                return;
            _validIds.Remove(steamID);
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            EChatMemberStateChange stateChange = (EChatMemberStateChange)callback.m_rgfChatMemberStateChange;
            ulong userSteamID = callback.m_ulSteamIDUserChanged;
            switch (stateChange)
            {
                case EChatMemberStateChange.k_EChatMemberStateChangeEntered:
                    AddValidSteamID(userSteamID);
                    break;

                case EChatMemberStateChange.k_EChatMemberStateChangeLeft:
                case EChatMemberStateChange.k_EChatMemberStateChangeKicked:
                case EChatMemberStateChange.k_EChatMemberStateChangeBanned:
                case EChatMemberStateChange.k_EChatMemberStateChangeDisconnected:
                    RemoveValidSteamID(userSteamID);
                    break;

                default:
                    break;
            }
        }
    }
}