
<p align="center">
  <a href="#"><img src="https://raw.githubusercontent.com/HerpDerpinstine/DDSS.LobbyGuard/main/Docs/Logo.png"></a>
</p>

---

Serverside Anti-Cheat and Security for [Dale & Dawson Stationery Supplies](https://store.steampowered.com/app/2920570/Dale__Dawson_Stationery_Supplies/)

- Protects Lobbies and Users from Malicious Exploits
- Fixes Disruptive Gameplay issues
- Serverside Authoritative, Only the Lobby Host needs to have it installed
- Seamless Integration with Gameplay
- Persistent Blacklist with Json File Saving
- Extended Invite Codes to avoid Generation Conflicts
- Extended Gameplay Settings

* Settings Menu can be found either on the Main Menu or while in a Lobby
* Persistent Blacklist Database is found in ``UserData/LobbyGuard/Blacklist.json``

* Discord: https://discord.gg/JDw423Wskf

---

### REQUIREMENTS:

- [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) v0.6.5 or higher.

---

### INSTALLATION:

1) Install [MelonLoader](https://github.com/LavaGang/MelonLoader/releases) v0.6.5 or higher.
4) Download [DDSS.LobbyGuard](https://github.com/HerpDerpinstine/DDSS.LobbyGuard/releases) from Releases.
5) Place ``DDSS.LobbyGuard.dll`` in the ``Mods`` folder of your Game's Install Folder.
6) Start the Game.

---

### PERSISTENT BLACKLIST:

- To remove someone from the database delete their entry from `{` to `},`

- Database Entries are layed out in the following structure:

| Value | Description |
| - | - |
| Timestamp | (UTC) Date and Time of when this User was blacklisted |
| SteamID | The SteamID64 of the User |
| Name | The Name of the User at the time of blacklist |

---

### LICENSING & CREDITS:

DDSS.LobbyGuard is licensed under the Apache License, Version 2.0. See [LICENSE](https://github.com/HerpDerpinstine/DDSS.LobbyGuard/blob/main/LICENSE.md) for the full License.

DDSS.LobbyGuard is not affiliated with Striped Panda Studios or [Dale & Dawson Stationery Supplies](https://store.steampowered.com/app/2920570/Dale__Dawson_Stationery_Supplies/) in any way.