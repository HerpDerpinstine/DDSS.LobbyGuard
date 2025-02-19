using HarmonyLib;
using Il2Cpp;
using Il2CppPlayer.Lobby;
using Il2CppTMPro;
using UnityEngine;

namespace DDSS_LobbyGuard.CharacterNamesInLobby.Patches
{
    [HarmonyPatch]
    internal class Patch_LobbyTab
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LobbyTab), nameof(LobbyTab.UpdateChat))]
        private static bool UpdateChat_Prefix(LobbyTab __instance)
        {
            if (!ModuleConfig.Instance.CharacterNamesInTextChat.Value)
                return true;

            // Get Chat TMP
            TextMeshProUGUI textMeshProUGUI = __instance.chatText;

            // Clear Chat
            textMeshProUGUI.text = string.Empty;

            // Iterate through Messages
            for (int i = Mathf.Min(LobbyManager.instance.chatMessages.Count - 1, __instance.maxMessages); i >= 0; i--)
            {
                // Get Message
                Il2CppSystem.ValueTuple<LobbyPlayer, string, string> valueTuple = LobbyManager.instance.chatMessages[i];

                // Get Player
                LobbyPlayer player = valueTuple.Item1;

                // Get Player Color
                Color color = LobbyManager.instance.playerColors[player.playerColorIndex];

                // Get Player Name
                string name = string.Concat(
                [
                    "<color=#",
                    ColorUtility.ToHtmlStringRGB(color),
                    ">",
                    player.username,
                    "</color>"
                ]);

                // Get Message
                string text = "[" + valueTuple.Item3 + "]";

                // Add to Chat TMP
                textMeshProUGUI.text = string.Concat([textMeshProUGUI.text, "<i>", text, "</i> <b>", name, "</b> : ", valueTuple.Item2, "\n"]);
            }

            // Prevent Original
            return false;
        }
    }
}
