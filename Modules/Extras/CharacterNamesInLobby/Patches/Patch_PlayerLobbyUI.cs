using HarmonyLib;
using Il2CppTMPro;
using Il2CppUI.Tabs.LobbyTab;
using UnityEngine;

namespace DDSS_LobbyGuard.Extras.CharacterNamesInLobby.Patches
{
    [HarmonyPatch]
    internal class Patch_PlayerLobbyUI
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.Update))]
        private static void Update_Postfix(PlayerLobbyUI __instance)
        {
            if (__instance.playerName == null)
                return;

            // Check Config
            if (ModuleConfig.Instance.CharacterNamesInPlayerList.Value)
            {
                TextMeshProUGUI characterName = ModuleMain.GetLobbyUICharacterName(__instance);
                if ((characterName != null)
                    && !characterName.WasCollected)
                    characterName.text = __instance.lobbyPlayer.Networkusername;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerLobbyUI), nameof(PlayerLobbyUI.Start))]
        private static void Start_Postfix(PlayerLobbyUI __instance)
        {
            if (__instance.playerName == null)
                return;

            // Check Config
            if (ModuleConfig.Instance.CharacterNamesInPlayerList.Value)
            {
                // Clone playerName
                GameObject characterNameObj = GameObject.Instantiate(__instance.playerName.gameObject, __instance.playerName.transform.parent);
                characterNameObj.name = "CharacterName";

                // Set characterName
                TextMeshProUGUI characterName = characterNameObj.GetComponent<TextMeshProUGUI>();
                characterName.text = __instance.lobbyPlayer.Networkusername;
                characterName.color = Color.black;
                ModuleMain.AddLobbyUICharacterName(__instance, characterName);

                // Move characterName
                Vector3 localPos = __instance.playerName.transform.localPosition;
                localPos.y = 7;
                characterName.transform.localPosition = localPos;

                // Move playerName
                localPos.y = -7;
                __instance.playerName.transform.localPosition = localPos;
            }
        }
    }
}
